using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using MWW.Collections;

namespace PI8583.Network
{
    public class ReactorHandler
    {
        public EndPoint RemoteEndPoint { get; set; }
        public override string ToString()
        {
            try
            {
                return string.Format("{0}\t{1}\t{2}", RemoteEndPoint, LatestResponseTime, (DateTime.Now - LatestResponseTime).TotalSeconds);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private bool _isSending = false;
        private Socket _handler;

        protected Queue<MessageBlock> MsgQueue
        {
            get;
            set;
        }

        public Socket Handler
        {
            protected get { return _handler; }
            set
            {
                _handler = value;
                try
                {
                    RemoteEndPoint = value.RemoteEndPoint;
                }
                catch (Exception)
                {

                }
            }
        }

        public DateTime LatestResponseTime { get; set; }

        public ReactorHandler()
        {
            this.MsgQueue = new Queue<MessageBlock>();
            this.LatestResponseTime = DateTime.Now;
        }
        public int Send(byte[] bytes)
        {
            int result;
            try
            {
                lock (this.MsgQueue)
                {
                    this.MsgQueue.Enqueue(new MessageBlock(bytes));
                    this.doSend();
                    result = 0;
                }
            }
            catch (System.Exception)
            {
                result = -1;
            }
            return result;
        }
        private void doSend()
        {
            if (!this._isSending)
            {
                while (this.MsgQueue.Count != 0)
                {
                    MessageBlock messageBlock = this.MsgQueue.Dequeue();
                    if (!messageBlock.IsEmpty)
                    {
                        if (this.Handler.Connected)
                        {
                            this._isSending = true;
                            this.Handler.BeginSend(messageBlock.Data, messageBlock.Index, messageBlock.RemainLength, SocketFlags.None, new System.AsyncCallback(this.OnSended), messageBlock);
                            return;
                        }
                    }
                }
                this._isSending = false;
            }
        }
        private void OnSended(System.IAsyncResult result)
        {
            lock (this.MsgQueue)
            {
                try
                {
                    if (!Handler.Connected)
                        return;

                    int length = this.Handler.EndSend(result);
                    MessageBlock messageBlock = (MessageBlock)result.AsyncState;
                    messageBlock.Read(length);
                    this.MsgQueue.Enqueue(messageBlock);
                    this._isSending = false;
                    this.doSend();
                }
                catch (System.Exception)
                {
                }
            }
        }
        public virtual void OnReceived(byte[] data)
        {
        }
        public virtual void OnConnected()
        {
        }
        protected virtual void OnDisconnected()
        {
        }


        public void Close()
        {
            try
            {
                if (this.Handler.Connected)
                    this.Handler.Close();
                OnDisconnected();
            }
            catch (Exception)
            {
            }
        }

        public IAsyncResult BeginReceive(byte[] array, int i, int maxReceiveSize, AsyncCallback asyncCallback, object asyncObject)
        {
            return this.Handler.BeginReceive(array, i, maxReceiveSize, SocketFlags.None, asyncCallback, asyncObject);
        }

        public int EndReceive(IAsyncResult result)
        {
            return this.Handler.EndReceive(result);
        }
    }
}