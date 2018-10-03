using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using PI8583.Protocal;
using PI8583.Network;

namespace PI8583
{
    public class PI8583Factory
    {
        private static ILog Logger = LogManager.GetLogger(typeof (PI8583Factory));

        public static byte[] Read(MemoryStream stream)
        {
            if (stream == null) 
                throw new ArgumentNullException("stream");
            if(stream.Length < 2) return null;

            var array = stream.ToArray();
            int length = array[0] * 0x0100 + array[1];
            if (array.Length - 2 < length)
            {
                return null;
            }

            stream.Position += length + 2;
            return array.Skip(2).Take(length).ToArray();
        }
        public static IRequest Create(byte[] bytes, I8638Context context)
        {
            var i8583 = new I8583(I8583.GetMessageType(bytes));
            Logger.Debug("长度匹配......");
            i8583.UnPack8583(bytes);
            Logger.Debug("解析协议完成");
            switch (i8583.MessageType)
            {
                case "0800":
                    return new SignInRequest(i8583){Context = context};
                case "0200":
                    return new DealRequest(i8583, bytes) { Context = context };
                case "0100":
                    return new PrePayRequest(i8583, bytes) { Context = context };
                //case "1000":
                //    return new QueryShopRequest(i8583, bytes) { Context = context };
                case "0400":
                    return new DealRequest_(i8583, bytes) { Context = context };
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
