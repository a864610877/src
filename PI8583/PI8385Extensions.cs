using System;
using System.IO;
using System.Linq;
using System.Text;
using PI8583.Protocal;

namespace PI8583
{
    public static class PI8385Extensions
    {

        public static void Display(string filename)
        {
            var bytes = File.ReadAllBytes(filename);

            Display(bytes);
        }

        public static void Display(byte[] bytes)
        {
            I8583 i8583 = new I8583(I8583.GetMessageType(bytes));
            i8583.UnPack8583(bytes.Skip(2).ToArray());
            Display(i8583);
        }

        public static string Display(this I8583 i8583)
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendLine("msgtype--" + i8583.MessageType);
            for (int i = 0; i < 64; i++)
            {
                if (i8583.Gettabx_flag(i) == "1")
                    buf.AppendLine((i + 1).ToString() + "--" + i8583.gettabx_data(i));
            }
            return buf.ToString();
        }
        public static void SetMac(this I8583 current, string packNo, string key)
        {
            var i8583 = current.Clone();
            i8583.settabx_data(63, "");
            var data = i8583.Pack8583(packNo);
            string mac = LCDES.MACEncrypt(data, key, 2);
            current.settabx_data(63, mac);
        }
    }
}