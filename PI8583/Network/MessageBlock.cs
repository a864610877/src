using System;
using System.Collections;
using System.Text;

namespace MWW.Collections
{
    /// <summary>
    /// 消息包
    /// </summary>
    public class MessageBlock
    {
        private int _index;
        private byte[] _data;

        #region Ctor
        /// <summary>
        /// 构造一个MessageBlock
        /// </summary>
        /// <param name="data">数据</param>
        public MessageBlock(byte[] data)
        {
            this.Write(data);
        }
        /// <summary>
        ///  构造一个空MessageBlock
        /// </summary>
        public MessageBlock()
        {
            this.Write(new byte[0] { });
        }
        #endregion
        #region Length
        /// <summary>
        /// 数据缓存长度
        /// </summary>
        public int Length
        {
            get
            {
                return this._data.Length;
            }
        }
        #endregion

        #region IsEmpty
        /// <summary>
        /// 消息包是否已经读完
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this._data.Length == this.Index;
            }
        }
        #endregion

        #region Remain Len

        /// <summary>
        /// 还剩下多少消息没读完
        /// </summary>
        public int RemainLength
        {
            get
            {
                return this._data.Length - this.Index;
            }
        }
        #endregion

        #region Data
        /// <summary>
        /// 消息包数据
        /// </summary>
        public byte[] Data
        {
            get
            {
                return this._data;
            }
        }
        #endregion

        #region Index
        /// <summary>
        /// 当前索引
        /// </summary>
        public int Index
        {
            get
            {
                return this._index;
            }
        }
        #endregion

        #region Read
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="length">要读取的长度，如果读取的长度大于剩余长度，则只读取剩余长度</param>
        /// <returns></returns>
        public byte[] Read(int length)
        {
            int remain_size = this._data.Length - this._index;
            length = (length > remain_size ? remain_size : length);

            byte[] bytes_ret = new byte[length];
            Buffer.BlockCopy(this._data, this._index, bytes_ret, 0, length);
            this._index += length;
            return bytes_ret;

        }
        #endregion

        
        #region Write
        /// <summary>
        /// 写数据，一量重写数据，则数据索引归0
        /// </summary>
        /// <param name="data">要写的数据</param>
        public void Write(byte[] data)
        {
            this._data = data;
            this._index = 0;
        }
        #endregion

        #region Rewrite
        /// <summary>
        /// 将索引的位置往回移动length
        /// </summary>
        /// <param name="length">要移动的偏移量</param>
        public void Rewrite(int length)
        {
            if (this._index > length)
            {
                this._index -= length;
            }
            else
            {
                this._index = 0;
            }
        }
        #endregion

    }
}
