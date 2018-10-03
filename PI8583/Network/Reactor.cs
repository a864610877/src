using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using MWW.Collections;
using System.Diagnostics;
using System.Net;
using System.Timers;
using log4net;
using System.IO;
using System.Linq;

//namespace MWW.Net
//{
//    public class ReactorHandler<TypeParameter>
//    {
//        public TypeParameter Parameter;
//        private System.Net.Sockets.Socket handle;
//        private bool _isSending = false;
//        private Queue<MWW.Collections.MessageBlock> _msgQueue = new Queue<MWW.Collections.MessageBlock>();
//        protected Queue<MWW.Collections.MessageBlock> MsgQueue
//        {
//            [DebuggerStepThrough]
//            get
//            {
//                return this._msgQueue;
//            }
//            [DebuggerStepThrough]
//            set
//            {
//                this._msgQueue = value;
//            }
//        }
//        public Socket Handler
//        {
//            [DebuggerStepThrough]
//            get
//            {
//                return this.handle;
//            }
//            [DebuggerStepThrough]
//            set
//            {
//                this.handle = value;
//            }
//        }
//        public int Send( byte[] bytes )
//        {
//            try
//            {
//                lock ( this.MsgQueue )
//                {
//                    this.MsgQueue.Enqueue( new MessageBlock( bytes ) );
//                    this.doSend();
//                    return 0;
//                }
//            }
//            catch ( Exception )
//            {
//                return -1;
//            }
//        }
//        void doSend( )
//        {
//            if ( this._isSending )
//                return;
//            while ( this.MsgQueue.Count != 0 )
//            {
//                MessageBlock msg = (MessageBlock) this.MsgQueue.Dequeue();
//                if ( msg.IsEmpty )
//                    continue;
//                if ( this.Handler.Connected )
//                {
//                    this._isSending = true;
//                    this.Handler.BeginSend( msg.Data, msg.Index, msg.RemainLength, SocketFlags.None, new AsyncCallback( this.OnSended ), msg );
//                    return;
//                }
//            }
//            this._isSending = false;
//        }
//        void OnSended( IAsyncResult result )
//        {
//            lock ( this.MsgQueue )
//            {
//                try
//                {
//                    int sendCount = this.Handler.EndSend(result);
//                    MWW.Collections.MessageBlock msg = (MWW.Collections.MessageBlock)result.AsyncState;
//                    msg.Read(sendCount);

//                    this.MsgQueue.Enqueue(msg);
//                    this._isSending = false;
//                    this.doSend();
//                }
//                catch (Exception)
//                {
//                }
//            }
//        }
//        public virtual void OnException( SocketError error )
//        {
//        }
//        public virtual void OnReceived( byte[] data )
//        {
//        }
//        public virtual void OnConnected( )
//        {
//        }
//        public virtual void OnDisconnected( )
//        {
//        }
//        public virtual void OnClose( )
//        {
//        }
//    }

//    public class Reactor<TypeHandler, TypeParameter>
//        where TypeHandler : ReactorHandler<TypeParameter>, new()
//    {
//        private static int MaxReceiveSize = 1024 * 10;
//        private TypeHandler connHandler = null;
//        private TypeParameter Parameter;
//        private List<TypeHandler> _handlers = new List<TypeHandler>();
//        void RemoveHandler( TypeHandler handler )
//        {
//            //lock ( _handlers )
//            //{
//            //    if ( _handlers.Contains( handler ) )
//            //    {
//            //        _handlers.Remove( handler );
//            //    }
//            //}
//        }
//        void AddHandler( TypeHandler handler )
//        {
//            //lock ( _handlers )
//            //{
//            //    if ( !_handlers.Contains( handler ) )
//            //    {
//            //        _handlers.Add( handler );
//            //    }
//            //}
//        }
//        public TypeHandler ConnectHandler
//        {
//            get
//            {
//                return this.connHandler;
//            }
//        }
//        public void Close( )
//        {
//            //lock ( _handlers )
//            //{
//            //    foreach ( var client in _handlers )
//            //    {
//            //        try
//            //        {
//            //            client.Handler.Close();
//            //        }
//            //        catch ( Exception ex )
//            //        {
//            //            int i = 0;
//            //        }
//            //    }
//            //    _handlers.Clear();
//            //    if ( listener != null )
//            //    {
//            //        listener.Stop();
//            //    }
//            //}
//        }


//        public void Accept( int listenPort, TypeParameter parameter )
//        {
//            System.Net.IPEndPoint ipendpoint = new IPEndPoint( IPAddress.Parse( "0.0.0.0" ), listenPort );
//            this.Accept( ipendpoint, parameter );
//        }

//        TcpListener listener;
//        public void Accept( System.Net.IPEndPoint endpoint, TypeParameter parameter )
//        {
//            listener = new TcpListener( endpoint );
//            this.Parameter = parameter;
//            listener.Start();
//            listener.BeginAcceptSocket( new AsyncCallback( this.onAccepted ), listener );
//        }
//        public void Connect( System.Net.IPEndPoint remote )
//        {
//            Socket client = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
//            client.BeginConnect( remote.Address, remote.Port, new AsyncCallback( this.onConnected ), client );
//        }
//        protected virtual void onConnected( IAsyncResult result )
//        {
//            Socket client = (Socket) result.AsyncState;
//            try
//            {
//                client.EndConnect( result );
//            }
//            catch ( Exception )
//            {
//                return;
//            }
//            TypeHandler handler = new TypeHandler();
//            this.connHandler = handler;
//            handler.Handler = client;
//            handler.OnConnected();
//            byte[] bytes = new byte[MaxReceiveSize];
//            KeyValuePair<TypeHandler, byte[]> handlerDatas = new KeyValuePair<TypeHandler, byte[]>( handler, bytes );
//            System.Net.Sockets.SocketError error;
//            handler.Handler.BeginReceive( handlerDatas.Value, 0, Reactor<TypeHandler, TypeParameter>.MaxReceiveSize, SocketFlags.None, out error, new AsyncCallback( this.onReceived ), handlerDatas );
//        }
//        protected virtual void onAccepted( IAsyncResult result )
//        {
//            TcpListener listener = (TcpListener) result.AsyncState;
//            Socket sock = null;
//            try
//            {
//                if ( !listener.Server.IsBound )
//                {
//                    return;
//                }
//                sock = listener.EndAcceptSocket( result );
//            }
//            catch ( System.Net.Sockets.SocketException )
//            {
//                return;
//            }
//            TypeHandler client = new TypeHandler();
//            client.Parameter = this.Parameter;
//            client.Handler = sock;
//            byte[] bytes = new byte[MaxReceiveSize];
//            KeyValuePair<TypeHandler, byte[]> handlerBytes = new KeyValuePair<TypeHandler, byte[]>( client, bytes );
//            client.OnConnected();
//            try
//            {
//                client.Handler.BeginReceive( bytes, 0, Reactor<TypeHandler, TypeParameter>.MaxReceiveSize, SocketFlags.None, new AsyncCallback( this.onReceived ), handlerBytes );
//                AddHandler( client );
//            }
//            catch ( System.Net.Sockets.SocketException )
//            {
//                client.OnDisconnected();
//                client.Handler.Close();
//            }
//            catch ( Exception )
//            {
//            }
//            listener.BeginAcceptSocket( new AsyncCallback( this.onAccepted ), listener );
//        }
//        protected virtual void onReceived( IAsyncResult result )
//        {
//            KeyValuePair<TypeHandler, byte[]> handlerBytes = (KeyValuePair<TypeHandler, byte[]>) result.AsyncState;
//            SocketError error = SocketError.ConnectionAborted;
//            int recvCnt = 0;
//            TypeHandler client = handlerBytes.Key;
//            try
//            {
//                recvCnt = handlerBytes.Key.Handler.EndReceive( result, out error );
//            }
//            catch ( Exception )
//            {
//                client.OnDisconnected();
//                RemoveHandler( client );

//                client.Handler.Close();
//                return;
//            }


//            byte[] boundData = handlerBytes.Value;
//            if ( recvCnt > 0 )
//            {
//                byte[] recvData = new byte[recvCnt];
//                System.Buffer.BlockCopy( handlerBytes.Value, 0, recvData, 0, recvCnt );
//                client.OnReceived( recvData );
//                try
//                {
//                    client.Handler.BeginReceive( boundData, 0, Reactor<TypeHandler, TypeParameter>.MaxReceiveSize, SocketFlags.None, out error, new AsyncCallback( this.onReceived ), handlerBytes );
//                }
//                catch ( Exception )
//                {
//                    client.OnException( SocketError.Disconnecting );
//                }
//            }
//            else if ( recvCnt == 0 )
//            {
//                if ( this.connHandler == client )
//                    this.connHandler = null;
//                client.OnDisconnected();
//                RemoveHandler( client );

//                client.Handler.Close();
//            }
//            else
//            {
//                if ( this.connHandler == client )
//                    this.connHandler = null;
//                client.OnException( error );
//                client.Handler.Close();
//            }
//        }
//    }
//}

namespace PI8583.Network
{
    public class Reactor<TypeHandler, TType> where TypeHandler : ReactorHandler, new()
    {
        object MainThreadLocker = new object();
        public Reactor()
        {
            _handlers = new List<ReactorHandler>();
        }
        private static int MaxReceiveSize = 10240;
        private TcpListener listener;
        private readonly ILog logger = LogManager.GetLogger(typeof(Reactor<TypeHandler, TType>));
        Timer _timer;
        private List<ReactorHandler> _handlers;
        private bool _stoped;
        private void OnTimer(object state, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                _timer.Enabled = false;
                lock (MainThreadLocker)
                {
                    StringBuilder buffer = new StringBuilder();
                    buffer.AppendLine(string.Format("clients: {0}", _handlers.Count));
                    buffer.AppendLine(string.Format("latest updated: {0}", DateTime.Now));
                    foreach (var handler in _handlers.ToList())
                    {
                        buffer.AppendLine(handler.ToString());
                    }
                    File.WriteAllText(Path.Combine(Path.GetDirectoryName(typeof(ReactorHandler).Assembly.Location), "Status.txt"), buffer.ToString());
                    foreach (var handler in _handlers.ToList())
                    {
                        if (handler.LatestResponseTime.AddSeconds(10) < DateTime.Now)
                        {
                            CloseHandler(handler);
                        }
                    }
                }
            }
            finally
            {
                if (!_stoped)
                {
                    _timer.Enabled = true;
                }
            }
        }

        private void CloseHandler(ReactorHandler handler)
        {
            lock (MainThreadLocker)
            {
                _handlers.Remove(handler);
                handler.Close();
            }
        }

        public void Close()
        {
            _stoped = true;
            listener.Stop();
        }
        public void Accept(int listenPort, TType parameter)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), listenPort);
            this.Accept(endpoint);
        }
        public void Accept(IPEndPoint endpoint)
        {
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimer;
            _timer.Enabled = true;
            this.listener = new TcpListener(endpoint);
            this.listener.Start();
            this.listener.BeginAcceptSocket(new System.AsyncCallback(this.onAccepted), this.listener);
        }

        void CloseSocket(Socket socket)
        {
            try
            {
                if (socket != null)
                {
                    socket.Close();
                }
            }
            catch (Exception)
            {
            }
        }
        protected virtual void onAccepted(System.IAsyncResult result)
        {
            TcpListener tcpListener = (TcpListener)result.AsyncState;
            Socket handler = null;
            try
            {
                if (!tcpListener.Server.IsBound)
                {
                    return;
                }
                handler = tcpListener.EndAcceptSocket(result);

                ReactorHandler typeHandler = null;
                typeHandler = System.Activator.CreateInstance<TypeHandler>();
                typeHandler.Handler = handler;
                lock (MainThreadLocker)
                {
                    _handlers.Add(typeHandler);
                    byte[] array = new byte[MaxReceiveSize];
                    var keyValuePair = new KeyValuePair<ReactorHandler, byte[]>(typeHandler, array);
                    typeHandler.OnConnected();
                    typeHandler.BeginReceive(array, 0, MaxReceiveSize, this.onReceived, keyValuePair);
                }

            }
            catch (SocketException arg)
            {
                this.logger.Error("error in accepted\r\n" + arg);
                CloseSocket(handler);
            }
            tcpListener.BeginAcceptSocket(this.onAccepted, tcpListener);
        }
        protected virtual void onReceived(System.IAsyncResult result)
        {
            lock (MainThreadLocker)
            {
                var keyValuePair = (KeyValuePair<ReactorHandler, byte[]>)result.AsyncState;

                ReactorHandler reactorHandler = keyValuePair.Key;
                byte[] value = keyValuePair.Value;
                try
                {
                    if (!_handlers.Contains(reactorHandler))
                    {
                        return;
                    }
                    ReactorHandler key2 = keyValuePair.Key;
                    int readCount = key2.EndReceive(result);

                    if (readCount > 0)
                    {
                        byte[] array = new byte[readCount];
                        System.Buffer.BlockCopy(keyValuePair.Value, 0, array, 0, readCount);
                        reactorHandler.OnReceived(array);
                        reactorHandler.BeginReceive(value, 0, MaxReceiveSize, new System.AsyncCallback(this.onReceived), keyValuePair);
                    }
                    else
                    {
                        CloseHandler(reactorHandler);
                    }
                }
                catch (System.Exception arg)
                {
                    this.logger.Error("error in onReceived: \r\n" + arg);
                    CloseHandler(reactorHandler);
                }
            }
        }
    }
}
