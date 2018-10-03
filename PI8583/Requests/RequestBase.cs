using System;
using PI8583.Network;
using PI8583.Protocal;

namespace PI8583
{
    public abstract class RequestBase
    {
        internal I8583 _i8583;
        public RequestBase(I8583 i8583)
        {
            _i8583 = i8583;
        }
        public string ShopNameTo
        {
            set { _i8583.settabx_data(47, value); }
            get { return _i8583.gettabx_data(47); }
        }
        /// <summary>
        /// </summary>
        public string SerialNo
        {
            get { return _i8583.gettabx_data(10); }
        }

        public string Mac
        {
            set { _i8583.settabx_data(63, value); }
            get { return _i8583.gettabx_data(63); }
        }

        public string PosName
        {
            set { _i8583.settabx_data(40, value); }
            get { return _i8583.gettabx_data(40); }
        }

        public string Custom1
        {
            set { _i8583.settabx_data(59, value); }
            get { return _i8583.gettabx_data(59); }
        }

        public string ShopName
        {
            set { _i8583.settabx_data(41, value); }
            get { return _i8583.gettabx_data(41); }
        }

        public I8638Context Context { get; set; }

        public T GetResponse<T>(params int[] poss) where T : ResponseBase
        {
            for (int i = 0; i < poss.Length; i++)
            {
                poss[i]--;
            }
            var i8583 = new I8583(_i8583.MessageType);

            i8583.settabx_data(11, DateTime.Now.ToString("HHmmss"));
            i8583.settabx_data(12, DateTime.Now.ToString("MMdd"));
            i8583.settabx_data(31, "00");
            i8583.settabx_data(43, "0001");
            _i8583.CopyTo(i8583, poss);
             

            ResponseBase response = (ResponseBase)Activator.CreateInstance(typeof(T));
            response.Context = Context;
            response.I8583 = i8583;
            return (T)response;
        }
    }
}