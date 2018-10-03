using System.Text;

namespace PI8583.Protocal
{
    public struct D8583_
    {
        /// <summary>
        /// 标志位
        /// </summary>
        public string flag;
        /// <summary>
        /// 数据
        /// </summary>
        public string data;
        /// <summary>
        /// 数据域长度
        /// </summary>
        public int len;
        /// <summary>
        /// 实际长度
        /// </summary>
        public int len_act;
        /// <summary>
        /// 变长长度位数指示
        /// </summary>
        public int len_var;
        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType dataType;
        /// <summary>
        /// 压缩
        /// </summary>
        public bool C;
        public override string ToString()
        {
            return data;
        }
    };
    public struct D8583
    {
        /// <summary>
        /// 标志位
        /// </summary>
        public string flag;
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] data;
        /// <summary>
        /// 数据域长度
        /// </summary>
        public int len;
        /// <summary>
        /// 实际长度
        /// </summary>
        public int len_act;
        /// <summary>
        /// 变长长度位数指示
        /// </summary>
        public int len_var;
        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType dataType;
        /// <summary>
        /// 压缩
        /// </summary>
        public bool C;
        public override string ToString()
        {
            if (flag == "1")
                return I8583.Encoding.GetString(data);
            return string.Empty;
        }
    };
}