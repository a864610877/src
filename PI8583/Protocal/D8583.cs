using System.Text;

namespace PI8583.Protocal
{
    public struct D8583_
    {
        /// <summary>
        /// ��־λ
        /// </summary>
        public string flag;
        /// <summary>
        /// ����
        /// </summary>
        public string data;
        /// <summary>
        /// �����򳤶�
        /// </summary>
        public int len;
        /// <summary>
        /// ʵ�ʳ���
        /// </summary>
        public int len_act;
        /// <summary>
        /// �䳤����λ��ָʾ
        /// </summary>
        public int len_var;
        /// <summary>
        /// ��������
        /// </summary>
        public DataType dataType;
        /// <summary>
        /// ѹ��
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
        /// ��־λ
        /// </summary>
        public string flag;
        /// <summary>
        /// ����
        /// </summary>
        public byte[] data;
        /// <summary>
        /// �����򳤶�
        /// </summary>
        public int len;
        /// <summary>
        /// ʵ�ʳ���
        /// </summary>
        public int len_act;
        /// <summary>
        /// �䳤����λ��ָʾ
        /// </summary>
        public int len_var;
        /// <summary>
        /// ��������
        /// </summary>
        public DataType dataType;
        /// <summary>
        /// ѹ��
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