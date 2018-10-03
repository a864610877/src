using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PI8583.Protocal
{
    public abstract class I8583Initializer
    {
        static I8583Initializer()
        {
            Current = new I8583InitializerV1();
        }
        public static I8583Initializer Current { get; set; }
        internal static void InitTable(I8583 i8583, string messageType)
        {
            if (Current == null)
                throw new Exception("please init I8583Initializer::Current first");

            Current.InitTab(i8583, messageType);
        }

        protected abstract void InitTab(I8583 i8583, string messageType);
    }

    public class I8583InitializerV2 : I8583Initializer
    {
        protected override void InitTab(I8583 i8583, string messageType)
        {
            for (int i = 0; i < 64; i++)
            {
                i8583.settabx(i, "0", "", 0, 0, 0);
            }
            i8583.settabx_len(1, 19, 2, DataType.BCD);
            i8583.settabx_len(2, 6, DataType.BCD);
            i8583.settabx_len(3, 12, DataType.BCD);
            i8583.settabx_len(10, 6, DataType.BCD);
            i8583.settabx_len(11, 6, DataType.BCD);
            i8583.settabx_len(12, 4, DataType.BCD);
            i8583.settabx_len(13, 4, DataType.BCD);
            i8583.settabx_len(14, 4, DataType.BCD);
            i8583.settabx_len(21, 3, DataType.BCD);
            i8583.settabx_len(22, 3, DataType.BCD);
            i8583.settabx_len(24, 2, DataType.BCD);
            i8583.settabx_len(25, 2, DataType.BCD);
            i8583.settabx_len(31, 11, 2, DataType.BCD, true);
            i8583.settabx_len(34, 37, 2, DataType.BCD);
            i8583.settabx_len(35, 104, 3, DataType.BCD);
            i8583.settabx_len(36, 12, DataType.ASCII);
            i8583.settabx_len(37, 6, DataType.ASCII);
            i8583.settabx_len(38, 2, DataType.ASCII);
            i8583.settabx_len(40, 8, DataType.ASCII);
            i8583.settabx_len(41, 15, DataType.ASCII);
            i8583.settabx_len(43, 25, 2, DataType.ASCII);
            i8583.settabx_len(47, 62, 3, DataType.ASCII);
            i8583.settabx_len(48, 3, DataType.ASCII);
            i8583.settabx_len(51, 16, DataType.BINARY);
            i8583.settabx_len(52, 16, DataType.BCD);
            i8583.settabx_len(53, 20, 3, DataType.ASCII);
            i8583.settabx_len(59, 22, 3, DataType.BCD);
            i8583.settabx_len(60, 29, 3, DataType.BCD);

            switch (messageType)
            {
                case "0800":
                    i8583.settabx_len(61, 72, 3, DataType.BINARY, true);
                    break;
                default:
                    i8583.settabx_len(61, 60, 3, DataType.ASCII);
                    break;
            }
            //i8583.settabx_len(61, 48, 3, DataType.BINARY, true);
            i8583.settabx_len(62, 63, 3, DataType.ASCII);
            i8583.settabx_len(63, 16, DataType.BINARY);
        }
    }
    public class I8583InitializerV1 : I8583Initializer
    {
        protected override void InitTab(I8583 i8583, string messageType)
        {
            for (int i = 0; i < 64; i++)
            {
                i8583.settabx(i, "0", "", 0, 0, 0);
            }

            i8583.settabx_len(1, 19, 2, DataType.BCD);
            i8583.settabx_len(2, 6, DataType.BCD);
            i8583.settabx_len(3, 12, DataType.BCD);
            i8583.settabx_len(10, 6, DataType.BCD);
            i8583.settabx_len(11, 6, DataType.BCD);
            i8583.settabx_len(12, 4, DataType.BCD);
            i8583.settabx_len(13, 4, DataType.BCD);
            i8583.settabx_len(14, 4, DataType.BCD);
            i8583.settabx_len(21, 3, DataType.BCD);
            i8583.settabx_len(22, 3, DataType.BCD);
            i8583.settabx_len(24, 2, DataType.BCD);
            i8583.settabx_len(25, 2, DataType.BCD);
            i8583.settabx_len(31, 11, 2, DataType.BCD, true);
            i8583.settabx_len(34, 37, 2, DataType.BCD);
            i8583.settabx_len(35, 104, 3, DataType.BCD);
            i8583.settabx_len(36, 12, DataType.ASCII);
            i8583.settabx_len(37, 6, DataType.ASCII);
            i8583.settabx_len(38, 2, DataType.ASCII);
            i8583.settabx_len(40, 8, DataType.ASCII);
            i8583.settabx_len(41, 15, DataType.ASCII);
            i8583.settabx_len(43, 25, 2, DataType.ASCII);
            i8583.settabx_len(47, 62, 3, DataType.BCD);
            i8583.settabx_len(48, 3, DataType.ASCII);
            i8583.settabx_len(51, 16, DataType.BINARY);
            i8583.settabx_len(52, 16, DataType.BCD);
            i8583.settabx_len(53, 20, 3, DataType.ASCII);
            i8583.settabx_len(59, 22, 3, DataType.BCD);
            i8583.settabx_len(60, 29, 3, DataType.BCD);
            i8583.settabx_len(61, 48, 3, DataType.BINARY, true);
            i8583.settabx_len(62, 63, 3, DataType.ASCII);
            i8583.settabx_len(63, 16, DataType.BINARY);
        }
    }

    public class I8583
    {
        public I8583 Clone()
        {
            I8583 i8583 = new I8583(_messageType);
            i8583.TPDU = new byte[this.TPDU.Length];
            Array.Copy(this.TPDU, i8583.TPDU, this.TPDU.Length);
            for (int i = 0; i < 64; i++)
            {
                if (this.Gettabx_flag(i) == "1")
                    i8583.settabx_data(i, this.tab[i].data.ToList().ToArray());
            }
            return i8583;
        }
        public void CopyTo(I8583 i8583, params int[] idxs)
        {
            i8583.TPDU = this.TPDU;
            for (int i = 0; i < 64; i++)
            {
                if (Array.IndexOf(idxs, i) < 0) continue;
                if (this.Gettabx_flag(i) == "1")
                    i8583.settabx_data(i, this.tab[i].data.ToList().ToArray());
            }
        }
        public static Encoding Encoding = Encoding.GetEncoding("gb2312");
        public byte[] TPDU = new byte[5];

        #region 计算报文长度 GetDatagramLength
        /// <summary>
        /// 计算报文长度
        /// 暂时不用 POS设备传过来的报文包含两个字节的长度
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int GetDatagramLength(byte[] data)
        {
            //报文头11字节+消息类型2字节+位图8字节
            int length = 21;

            I8583 c = new I8583(GetMessageType(data));

            byte[] bitmap = new byte[8];
            string S_BitMap = "";
            int maplen = 8;
            for (int i = 13; i < 21; i++) //取16位位图
            {
                bitmap[i - 13] = data[i];
            }

            for (int i = 0; i < maplen; i++)
            {
                string T_BitMap = I8583.AcceptConvertString(System.Convert.ToString(bitmap[i]), 10, 2);
                S_BitMap = S_BitMap + T_BitMap;
            }

            int idx = 0;
            foreach (char bit in S_BitMap)
            {
                string tab = bit.ToString();
                int len = 0;
                if (length > data.Length)
                {
                    return -1;
                }

                if (tab == "1")
                {
                    switch (c.tab[idx].dataType)
                    {
                        case DataType.BCD:
                            #region BCD
                            if (c.tab[idx].len_var > 0)
                            {
                                if (c.tab[idx].len_var == 2)
                                {
                                    //两位变长
                                    len = int.Parse(I8583.SendConvertString(data[length].ToString(), 10, 16));
                                    length++;
                                }
                                else if (c.tab[idx].len_var == 3)
                                {
                                    //三位变长
                                    length++;
                                    len = int.Parse(I8583.SendConvertString(data[length].ToString(), 10, 16));
                                    length++;
                                }
                                if (len % 2 == 1)
                                {
                                    //如果数据长度为奇数 需要补一位
                                    len = len / 2 + 1;
                                }
                                else
                                {
                                    len = len / 2;
                                }
                            }
                            else
                            {
                                //定长字段
                                len = c.tab[idx].len / 2;
                                if (c.tab[idx].len % 2 == 1)
                                {
                                    len++;
                                }
                            }
                            length += len;
                            #endregion
                            break;
                        case DataType.ASCII:
                            #region ASCII
                            if (c.tab[idx].len_var > 0)
                            {
                                if (c.tab[idx].len_var == 2)
                                {
                                    //两位变长
                                    len = int.Parse(I8583.SendConvertString(data[length].ToString(), 10, 16));
                                    length++;
                                }
                                else if (c.tab[idx].len_var == 3)
                                {
                                    //三位变长
                                    length++;
                                    len = int.Parse(I8583.SendConvertString(data[length].ToString(), 10, 16));
                                    length++;
                                }
                            }
                            else
                            {
                                len = c.tab[idx].len;
                            }
                            length += len;
                            #endregion
                            break;
                        case DataType.BINARY:
                            #region BINARY
                            if (c.tab[idx].len_var > 0)
                            {
                                if (c.tab[idx].len_var == 2)
                                {
                                    //两位变长
                                    len = int.Parse(I8583.SendConvertString(data[length].ToString(), 10, 16));
                                    length++;
                                }
                                else if (c.tab[idx].len_var == 3)
                                {
                                    //三位变长
                                    length++;
                                    len = int.Parse(I8583.SendConvertString(data[length].ToString(), 10, 16));
                                    length++;
                                }
                                if (len % 2 == 1)
                                {
                                    //如果数据长度为奇数 需要补一位
                                    len = len / 2 + 1;
                                }
                                else
                                {
                                    len = len / 2;
                                }
                            }
                            else
                            {
                                //定长字段
                                len = c.tab[idx].len / 2;
                                if (c.tab[idx].len % 2 == 1)
                                {
                                    len++;
                                }
                            }
                            length += len;
                            #endregion
                            break;
                        default:
                            break;
                    }
                }
                idx++;
            }

            return length;
        }
        #endregion

        D8583[] tab = new D8583[64];
        public D8583[] Tabs { get { return tab; } }
        public I8583(string messageType)
        {
            _messageType = messageType;
            InitTab(messageType);
        }

        #region InitTab

        private void InitTab(string messageType)
        {
            I8583Initializer.InitTable(this, messageType);
        }
        #endregion

        #region tabx_Method
        public void settabx_flag(int i, string flag)
        {
            tab[i].flag = flag;
        }
        public string Gettabx_flag(int i)
        {
            return tab[i].flag;
        }
        public void settabx_data(int i, string data)
        {
            tab[i].flag = "1";
            tab[i].data = Encoding.GetBytes(data);
        }
        public void settabx_data(int i, byte[] data)
        {
            tab[i].flag = "1";
            tab[i].data = data;
        }
        public string gettabx_data(int i)
        {
            return Encoding.GetString(tab[i].data);
        }
        public void settabx_len(int i, int len, DataType dataType)
        {
            tab[i].len = len;
            tab[i].dataType = dataType;
        }
        public void settabx_len(int i, int len, int var, DataType dataType, bool C)
        {
            this.settabx_len(i, len, var, dataType);
            tab[i].C = C;
        }
        public void settabx_len(int i, int len, int var, DataType dataType)
        {
            tab[i].len = len;
            tab[i].len_var = var;
            tab[i].dataType = dataType;
        }
        public void settabx_len_act(int i, int len_act)
        {
            tab[i].len_act = len_act;
        }
        public void settabx_len_var(int i, int len_var)
        {
            tab[i].len_var = len_var;
        }
        public void settabx(int i, string flag, string data)
        {
            tab[i].flag = flag;
            tab[i].data = Encoding.GetBytes(data);
        }
        public void settabx(int i, string flag, string data, int len, int len_act, int len_var)
        {
            tab[i].flag = flag;
            tab[i].data = Encoding.GetBytes(data);
            tab[i].len = len;
            tab[i].len_act = len_act;
            tab[i].len_var = len_var;
        }
        #endregion

        #region MessageType

        private string _messageType;
        /// <summary>
        /// MyProperty
        /// </summary>
        public string MessageType
        {
            get { return _messageType; }
        }
        #endregion


        /// <summary>
        /// 发送BitMap转换
        /// </summary>
        /// <param name="value">需要转换的数据</param>
        /// <param name="fromBase">原进制</param>
        /// <param name="toBase">转换后进制</param>
        /// <returns></returns>
        public static string SendConvertString(string value, int fromBase, int toBase)
        {
            try
            {

                int intValue = Convert.ToInt32(value, fromBase);
                return Convert.ToString(intValue, toBase);
            }
            catch (Exception)
            {

                throw;
            }
        }
        byte[] ConcatLen(int len, int byteLen)
        {
            List<byte> bytes = new List<byte>();
            do
            {
                bytes.Add(Convert.ToByte((len % 0x100).ToString(), 16));
                len = len / 0x100;
            } while (len != 0);
            while (bytes.Count < byteLen)
            {
                bytes.Add(0);
            }
            bytes.Reverse();
            return bytes.ToArray();
        }
        /// <summary>
        /// 组包  
        /// </summary>
        /// <param name="msgType">消息类型char[4]</param>
        /// <returns></returns>
        public byte[] Pack8583(string msgType)
        {
            //BitMap
            byte[] BitMap = new byte[8];
            string[] ls_BitMap = new string[] { "", "", "", "", "", "", "", "" };
            for (int i = 0; i < 64; i++)
            {
                ls_BitMap[i / 8] += tab[i].flag;
            }
            for (int i = 0; i < 8; i++)
            {
                BitMap[i] = (byte)System.Convert.ToInt16(SendConvertString(ls_BitMap[i], 2, 10));
            }

            //报文头 6000030000602200000000
            //string s_head = "6000030000602200000000";
            //60 00 03 00 00 60 22 00 00 00 00
            byte[] b_head = new byte[11];
            b_head[0] = this.TPDU[0];
            b_head[1] = this.TPDU[1];
            b_head[2] = this.TPDU[2];
            b_head[3] = this.TPDU[3];
            b_head[4] = this.TPDU[4];
            b_head[5] = 96;
            b_head[6] = 34;
            b_head[7] = 0;
            b_head[8] = 0;
            b_head[9] = 0;
            b_head[10] = 0;

            byte[] b_msgType = new byte[2];
            b_msgType[0] = byte.Parse(SendConvertString(msgType.Substring(0, 2), 16, 10));
            b_msgType[1] = byte.Parse(SendConvertString(msgType.Substring(2), 16, 10));

            //扫描变长字段,付值时只需给数据付值，不须付长度
            for (int i = 0; i < 64; i++)
            {
                if (tab[i].len_var > 0 && tab[i].flag == "1")
                {
                    switch (tab[i].dataType)
                    {
                        case DataType.BCD:

                            break;
                        case DataType.ASCII:
                            //ASCII格式的后面再处理变长标记位
                            break;
                        case DataType.BINARY:
                            //BINARY格式的后面再处理变长标记位
                            break;
                        default:
                            break;
                    }
                }
            }
            //组报文
            ArrayList msgArr = new ArrayList();
            msgArr.Add(b_head);
            msgArr.Add(b_msgType);
            msgArr.Add(BitMap);

            int b_len;
            string tmp;
            for (int i = 0; i < 64; i++)
            {
                if (tab[i].flag == "1")
                {
                    byte[] b_var = null;

                    switch (tab[i].dataType)
                    {
                        case DataType.BCD:
                            // 计算长度
                            if (tab[i].len_var == 2)
                            {
                                //两位变长
                                if (tab[i].C)
                                {
                                    msgArr.Add(ConcatLen(tab[i].data.Length / 2, 1));
                                }
                                else
                                {
                                    msgArr.Add(ConcatLen(tab[i].data.Length, 1));
                                }
                            }
                            else if (tab[i].len_var == 3)
                            {
                                //三位变长
                                if (tab[i].C)
                                {
                                    //长度减半 
                                    msgArr.Add(ConcatLen(tab[i].data.Length / 2, 2));
                                }
                                else
                                {
                                    msgArr.Add(ConcatLen(tab[i].data.Length, 2));
                                }
                            }

                            #region BCD
                            b_len = tab[i].data.Length / 2;
                            if (tab[i].data.Length % 2 == 1)
                            {
                                b_len++;
                            }
                            byte[] b_BCD = new byte[b_len];

                            for (int j = 0; j < b_len; j++)
                            {
                                if (tab[i].data.Length - j * 2 > 1)
                                {
                                    //每次取两位
                                    //tmp = tab[i].data.Substring(j * 2, 2);
                                    tmp = Encoding.ASCII.GetString(tab[i].data.Skip(j * 2).Take(2).ToArray());
                                }
                                else
                                {
                                    //如果最后不足两位，则后补‘0’
                                    //tmp = tab[i].data.Substring(j * 2) + "0";
                                    tmp = Encoding.ASCII.GetString(tab[i].data.Skip(j * 2).Take(2).ToArray()) + "0";
                                }
                                b_BCD[j] = byte.Parse(SendConvertString(tmp, 16, 10));
                            }
                            msgArr.Add(b_BCD);
                            #endregion
                            break;
                        case DataType.ASCII:
                            #region ASCII
                            //变长标记位
                            if (tab[i].len_var > 0)
                            {
                                if (tab[i].len_var == 2)
                                {
                                    //两位变长
                                    b_var = new byte[1];
                                    tmp = tab[i].data.Length.ToString().PadLeft(2, '0');
                                    b_var[0] = byte.Parse(SendConvertString(tmp, 16, 10));
                                }
                                else if (tab[i].len_var == 3)
                                {
                                    //三位变长
                                    //b_var = new byte[2];
                                    //b_var[0] = 0x00;
                                    //tmp = tab[i].data.Length.ToString().PadLeft(2, '0');
                                    //b_var[1] = byte.Parse(SendConvertString(tmp, 16, 10));

                                    b_var = new byte[2];
                                    b_var[0] = FromBcd((byte)(tab[i].data.Length / 100));
                                    b_var[1] = FromBcd((byte)(tab[i].data.Length % 100));
                                }
                            }

                            //数据体 
                            if (b_var != null)
                            {
                                //如果存在变长标记位，则保存
                                msgArr.Add(b_var);
                            }
                            msgArr.Add(tab[i].data);
                            #endregion
                            break;
                        case DataType.BINARY:
                            #region BINARY
                            //变长标记位
                            if (tab[i].len_var > 0)
                            {
                                if (tab[i].len_var == 2)
                                {
                                    //两位变长
                                    b_var = new byte[1];
                                    if (tab[i].C)
                                    {
                                        tmp = (tab[i].data.Length / 2).ToString().PadLeft(2, '0');
                                    }
                                    else
                                    {
                                        tmp = tab[i].data.Length.ToString().PadLeft(2, '0');
                                    }
                                    b_var[0] = byte.Parse(SendConvertString(tmp, 16, 10));
                                }
                                else if (tab[i].len_var == 3)
                                {
                                    //三位变长
                                    b_var = new byte[2];
                                    b_var[0] = 0x00;

                                    if (tab[i].C)
                                    {
                                        tmp = (tab[i].data.Length / 2).ToString().PadLeft(2, '0');
                                    }
                                    else
                                    {
                                        tmp = tab[i].data.Length.ToString().PadLeft(2, '0');
                                    }
                                    b_var[1] = byte.Parse(SendConvertString(tmp, 16, 10));
                                }
                            }

                            b_len = tab[i].data.Length / 2;
                            if (b_len % 2 == 1)
                            {
                                b_len++;
                            }


                            byte[] b_BINARY = new byte[b_len];

                            for (int j = 0; j < b_len; j++)
                            {
                                if (tab[i].data.Length - j * 2 > 1)
                                {
                                    //每次取两位
                                    //tmp = tab[i].data.Substring(j * 2, 2);
                                    tmp = Encoding.ASCII.GetString(tab[i].data.Skip(j * 2).Take(2).ToArray());
                                }
                                else
                                {
                                    //如果最后不足两位，则后补‘0’
                                    //tmp = tab[i].data.Substring(j * 2) + "0";
                                    tmp = Encoding.ASCII.GetString(tab[i].data.Skip(j * 2).Take(2).ToArray()) + "0"; ;
                                }
                                b_BINARY[j] = byte.Parse(SendConvertString(tmp, 16, 10));
                            }

                            if (b_var != null)
                            {
                                //如果存在变长标记位，则保存
                                msgArr.Add(b_var);
                            }
                            msgArr.Add(b_BINARY);
                            #endregion
                            break;
                        default:
                            break;
                    }
                }
            }

            //实际报文长度
            int length = 0;
            foreach (byte[] b in msgArr)
            {
                length += b.Length;
            }

            byte[] B_Msg = new byte[length];
            int n = 0;
            foreach (byte[] Msaarr in msgArr)
            {
                for (int i = 0; i < Msaarr.Length; i++)
                {
                    B_Msg[n] = Msaarr[i];
                    n++;
                }
            }

            //返回的内容前面增加两个字节的长度位
            byte[] ret_buffer = new byte[B_Msg.Length + 2];
            Array.Copy(B_Msg, 0, ret_buffer, 2, B_Msg.Length);

            int ret_len = B_Msg.Length;
            ret_buffer[0] = (byte)(ret_len / 0x100);
            ret_buffer[1] = (byte)(ret_len % 0x100);

            return ret_buffer;
        }


        /// <summary>
        /// 接收报文BITMAP转换
        /// </summary>
        /// <param name="value">需要转换的数据</param>
        /// <param name="fromBase">转换前进制</param>
        /// <param name="toBase">转换后进制</param>
        /// <returns></returns>
        public static string AcceptConvertString(string value, int fromBase, int toBase)
        {
            int intValue = Convert.ToInt32(value, fromBase);
            string S_ConvertString = Convert.ToString(intValue, toBase);
            S_ConvertString = Convert.ToInt32(S_ConvertString).ToString("D8");
            return S_ConvertString;
        }

        public static string aaa(byte[] buffer)
        {
            string ret = "";
            for (int i = 0; i < buffer.Length; i++)
            {
                ret += SendConvertString(buffer[i].ToString(), 10, 16).PadLeft(2, '0') + " ";
            }
            return ret;
        }
        public static string GetMessageType(byte[] bytes)
        {
            return SendConvertString(bytes[11].ToString(), 10, 16).PadLeft(2, '0') + SendConvertString(bytes[12].ToString(), 10, 16).PadLeft(2, '0');
        }
        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="B_Msg">字节报文：11字节报文头+2字节消息类型+8字节位图+n数据元</param>
        public void UnPack8583(byte[] B_Msg)
        {
            if (string.IsNullOrEmpty(_messageType))
            {
                _messageType = GetMessageType(B_Msg);
                InitTab(_messageType);
            }
            //报文不合法
            if (B_Msg.Length < 21)
            {
                tab[0].data = Encoding.GetBytes("Error:Packed lenth<44");
                return;
            }
            Array.Copy(B_Msg, 0, this.TPDU, 0, 5);
            byte[] bitmap = new byte[8];
            string S_BitMap = "";
            int maplen = 8;
            for (int i = 13; i < 21; i++) //取16位位图 8个字节
            {
                bitmap[i - 13] = B_Msg[i];
            }

            for (int i = 0; i < maplen; i++)
            {
                string T_BitMap = I8583.AcceptConvertString(System.Convert.ToString(bitmap[i]), 10, 2);
                S_BitMap = S_BitMap + T_BitMap;
            }

            //取MessageType 2个字节
            byte[] msgType = new byte[2];
            msgType[0] = B_Msg[11];
            msgType[1] = B_Msg[12];
            //_messageType = SendConvertString(msgType[0].ToString(), 10, 16).PadLeft(2, '0') + SendConvertString(msgType[1].ToString(), 10, 16).PadLeft(2, '0');

            //解读报文
            int n = 0;
            int ptr = 21;
            foreach (char C_BitMap in S_BitMap)
            {
                string flag = Convert.ToString(C_BitMap);
                if (flag == "1")
                {
                    try
                    {
                        List<byte> data = new List<byte>();
                        int len = 0;
                        int m_len = 0;
                        switch (tab[n].dataType)
                        {
                            case DataType.BCD:
                                #region BCD
                                if (tab[n].len_var > 0)
                                {
                                    //变长字段
                                    if (tab[n].len_var == 2)
                                    {
                                        //两位变长
                                        len = int.Parse(SendConvertString(B_Msg[ptr].ToString(), 10, 16));
                                        if (tab[n].C)
                                        {
                                            len = len * 2;
                                        }
                                        ptr++;
                                    }
                                    else if (tab[n].len_var == 3)
                                    {
                                        //三位变长
                                        //第一位长度
                                        len = int.Parse(SendConvertString(B_Msg[ptr].ToString(), 10, 16));
                                        len = len * 100;
                                        ptr++;
                                        //第二位是具体的长度
                                        len = len + int.Parse(SendConvertString(B_Msg[ptr].ToString(), 10, 16));
                                        if (tab[n].C)
                                        {
                                            len = len * 2;
                                        }
                                        ptr++;
                                    }
                                    m_len = len;
                                    if (len % 2 == 1)
                                    {
                                        //如果数据长度为奇数 需要补一位
                                        len = len / 2 + 1;
                                    }
                                    else
                                    {
                                        len = len / 2;
                                    }
                                }
                                else
                                {
                                    //定长字段
                                    m_len = tab[n].len;
                                    len = tab[n].len / 2;
                                    if (tab[n].len % 2 == 1)
                                    {
                                        len++;
                                    }
                                }
                                for (int i = 0; i < len; i++)
                                {
                                    var s = SendConvertString(B_Msg[ptr].ToString(), 10, 16).PadLeft(2, '0');
                                    data.Add(Convert.ToByte(s[0]));
                                    data.Add(Convert.ToByte(s[1]));
                                    ptr++;
                                }
                                //去掉自动补充的位（‘0’）
                                data = data.Take(m_len).ToList();
                                this.settabx_data(n, data.ToArray());
                                #endregion
                                break;
                            case DataType.ASCII:
                                #region ASCII
                                if (tab[n].len_var > 0)
                                {
                                    //变长字段
                                    if (tab[n].len_var == 2)
                                    {
                                        //两位变长
                                        len = int.Parse(SendConvertString(B_Msg[ptr].ToString(), 10, 16));
                                        if (tab[n].C)
                                        {
                                            len = len * 2;
                                        }
                                        ptr++;
                                    }
                                    else if (tab[n].len_var == 3)
                                    {
                                        //三位变长
                                        //第一位长度
                                        len = int.Parse(SendConvertString(B_Msg[ptr].ToString(), 10, 16));
                                        len = len * 100;
                                        ptr++;
                                        //第二位是具体的长度
                                        len = len + int.Parse(SendConvertString(B_Msg[ptr].ToString(), 10, 16));
                                        if (tab[n].C)
                                        {
                                            len = len * 2;
                                        }
                                        ptr++;
                                    }
                                    m_len = len;
                                    ////变长字段
                                    //if (tab[n].len_var == 2)
                                    //{
                                    //    //两位变长
                                    //    len = int.Parse(SendConvertString(B_Msg[ptr++].ToString()+B_Msg[ptr].ToString(), 10, 16));
                                    //    ptr++;
                                    //}
                                    //else if (tab[n].len_var == 3)
                                    //{
                                    //    //三位变长
                                    //    //第一位是长度
                                    //    len = int.Parse(SendConvertString(B_Msg[ptr++].ToString() +B_Msg[ptr++].ToString()+B_Msg[ptr].ToString(), 10, 16));
                                    //    len = len * 100;
                                    //    ptr++;
                                    //    //第二位是具体的长度
                                    //    len = len + int.Parse(SendConvertString(B_Msg[ptr].ToString(), 10, 16));
                                    //    ptr++;
                                    //}
                                }
                                else
                                {
                                    len = tab[n].len;
                                }
                                for (int i = 0; i < len; i++)
                                {
                                    //byte[] m_b = new byte[1];
                                    //m_b[0] = B_Msg[ptr];
                                    //data += System.Text.Encoding.Default.GetString(m_b);
                                    data.Add(B_Msg[ptr]);
                                    ptr++;
                                }
                                this.settabx_data(n, data.ToArray());
                                #endregion
                                break;
                            case DataType.BINARY:
                                #region BINARY
                                if (tab[n].len_var > 0)
                                {
                                    //变长字段
                                    if (tab[n].len_var == 2)
                                    {
                                        //两位变长
                                        len = int.Parse(SendConvertString(B_Msg[ptr].ToString(), 10, 16));
                                        ptr++;
                                    }
                                    else if (tab[n].len_var == 3)
                                    {
                                        //三位变长
                                        //第一位是0x00
                                        len = int.Parse(SendConvertString(B_Msg[ptr].ToString(), 10, 16));
                                        len = len * 100;
                                        ptr++;
                                        //第二位是具体的长度
                                        len = len + int.Parse(SendConvertString(B_Msg[ptr].ToString(), 10, 16));
                                        ptr++;
                                    }
                                    m_len = len;
                                    if (len % 2 == 1)
                                    {
                                        //如果数据长度为奇数 需要补一位
                                        len = len / 2 + 1;
                                    }
                                    else
                                    {
                                        len = len / 2;
                                    }
                                }
                                else
                                {
                                    //定长字段
                                    len = tab[n].len / 2;
                                    if (tab[n].len % 2 == 1)
                                    {
                                        len++;
                                    }
                                }
                                for (int i = 0; i < len; i++)
                                {
                                    //data += SendConvertString(B_Msg[ptr].ToString(), 10, 16).PadLeft(2, '0');

                                    var s = SendConvertString(B_Msg[ptr].ToString(), 10, 16).PadLeft(2, '0');
                                    data.Add(Convert.ToByte(s[0]));
                                    data.Add(Convert.ToByte(s[1]));
                                    ptr++;
                                }
                                if (m_len > 0)
                                {
                                    //data = data.Substring(0, m_len);
                                    data = data.Take(m_len).ToList();
                                }
                                this.settabx_data(n, data.ToArray());
                                #endregion
                                break;
                            default:
                                break;
                        }
                    }
                    catch
                    {
                        tab[0].data = Encoding.GetBytes("Error:Packed tab" + n.ToString());
                        return;
                    }
                }
                n++;
            }
        }
        static byte FromBcd(byte d)
        {
            return Convert.ToByte(d.ToString(), 16);
        }
    }
}