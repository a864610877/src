using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace PI8583.Protocal
{
    public class LCDES
    {
        #region 加密原函数 DesEncrypt
        /// <summary>
        /// 加密原函数, 返回为16进制字符串
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string DesEncrypt(string pToEncrypt, byte[] key)
        {
            string sReturn = "";
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.None;
                ICryptoTransform myICT = des.CreateEncryptor(key, new byte[8]);

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, myICT, CryptoStreamMode.Write);

                byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                cs.Close();
                byte[] bTmp = ms.ToArray();
                ms.Close();

                //转换成16进制
                for (int i = 0; i < bTmp.Length; i++)
                {
                    sReturn = sReturn + AcceptConvertString(bTmp[i].ToString("D2"), 10, 16);
                }
            }
            catch (Exception)
            {

            }
            return sReturn;
        }
        #endregion

        #region 解密原函数 DesDecrypt
        /// <summary>
        /// 解密原函数
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string DesDecrypt(string pToDecrypt, string sKey)
        {
            string sReturn = "";
            try
            {
                byte[] key = ASCIIEncoding.ASCII.GetBytes(sKey);

                string sData = pToDecrypt;

                byte[] inputByteArray = new byte[sData.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(sData.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.None;

                ICryptoTransform myICT = des.CreateDecryptor(key, new byte[8]);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, myICT, CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                byte[] bytes = ms.ToArray();
                sReturn = System.Text.Encoding.Default.GetString(bytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return sReturn;
        }
        /// <summary>
        /// 解密原函数
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static Byte[] DesDecryptB(string pToDecrypt, string sKey)
        {
            try
            {
                byte[] key = ASCIIEncoding.ASCII.GetBytes(sKey);

                string sData = pToDecrypt;

                byte[] inputByteArray = new byte[sData.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(sData.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                return DesDecryptB(inputByteArray, key);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new byte[0];
        }

        public static byte[] DesDecryptB(byte[] inputByteArray, byte[] key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.None;

            ICryptoTransform myICT = des.CreateDecryptor(key, new byte[8]);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, myICT, CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        public static byte[] ToHexArray(string pToDecrypt)
        {
            List<byte> array = new List<byte>();
            for (int i = 0; i < pToDecrypt.Length / 2; i++)
            {
                array.Add(Convert.ToByte(pToDecrypt.Substring(i * 2, 2), 16));
            }
            return array.ToArray();
        }
        #endregion

        #region 进制转换函数 AcceptConvertString
        /// <summary> 
        /// 进制转换函数 
        /// </summary> 
        /// <param name="value">需要转换的数据 </param> 
        /// <param name="fromBase">转换前进制 </param> 
        /// <param name="toBase">转换后进制 </param> 
        /// <returns> </returns> 
        public static string AcceptConvertString(string value, int fromBase, int toBase)
        {
            int intValue = Convert.ToInt32(value, fromBase);

            string S_ConvertString = Convert.ToString(intValue, toBase);
            if (intValue < 10)
            {
                S_ConvertString = Convert.ToInt32(S_ConvertString).ToString("D2");
            }
            // 
            S_ConvertString = S_ConvertString.ToUpper();
            if (S_ConvertString.Length == 1)
            {
                S_ConvertString = "0" + S_ConvertString;
            }
            return S_ConvertString;
        }
        #endregion

        #region MACEncrypt
        /*
            POS终端采用ＥＣＢ的加密方式，简述如下：
            a)  将欲发送给POS中心的消息中，从消息类型（MTI）到63域之间的部分构成MAC ELEMEMENT BLOCK （MAB）。
            b)  对MAB，按每8个字节做异或（不管信息中的字符格式），如果最后不满8个字节，则添加“0X00”。
            示例	：
            MAB = M1 M2 M3 M4
            其中：	
            M1 = MS11 MS12 MS13 MS14 MS15 MS16 MS17 MS18
            M2 = MS21 MS22 MS23 MS24 MS25 MS26 MS27 MS28
            M3 = MS31 MS32 MS33 MS34 MS35 MS36 MS37 MS38
            M4 = MS41 MS42 MS43 MS44 MS45 MS46 MS47 MS48

            按如下规则进行异或运算：
 			            MS11 MS12 MS13 MS14 MS15 MS16 MS17 MS18
            XOR）			MS21 MS22 MS23 MS24 MS25 MS26 MS27 MS28
            ---------------------------------------------------
            TEMP BLOCK1 =	TM11 TM12 TM13 TM14 TM15 TM16 TM17 TM18

            然后，进行下一步的运算：
            TM11 TM12 TM13 TM14 TM15 TM16 TM17 TM18
            XOR）			MS31 MS32 MS33 MS34 MS35 MS36 MS37 MS38
            ---------------------------------------------------
            TEMP BLOCK2 =	TM21 TM22 TM23 TM24 TM25 TM26 TM27 TM28

            再进行下一步的运算：
            TM21 TM22 TM23 TM24 TM25 TM26 TM27 TM28
            XOR）			MS41 MS42 MS43 MS44 MS45 MS46 MS47 MS48
            ---------------------------------------------------
            RESULT BLOCK =	TM31 TM32 TM33 TM34 TM35 TM36 TM37 TM38
            		
            c)  将异或运算后的最后8个字节（RESULT BLOCK）转换成16 个HEXDECIMAL：
            RESULT BLOCK = TM31 TM32 TM33 TM34 TM35 TM36 TM37 TM38
	                     = TM311 TM312 TM321 TM322 TM331 TM332 TM341 TM342 ||
		                   TM351 TM352 TM361 TM362 TM371 TM372 TM381 TM382

            d)  取前8 个字节用MAK加密：
            ENC BLOCK1 = eMAK（TM311 TM312 TM321 TM322 TM331 TM332 TM341 TM342）
			            = EN11 EN12 EN13 EN14 EN15 EN16 EN17 EN18

            e)  将加密后的结果与后8 个字节异或：
            EN11  EN12  EN13  EN14  EN15  EN16  EN17  EN18
            XOR）     	TM351 TM352 TM361 TM362 TM371 TM372 TM381 TM382
            ------------------------------------------------------------
            TEMP BLOCK=	TE11  TE12  TE13  TE14  TE15  TE16  TE17  TE18

            f)  用异或的结果TEMP BLOCK 再进行一次单倍长密钥算法运算。
            ENC BLOCK2 = eMAK（TE11 TE12 TE13 TE14 TE15 TE16 TE17 TE18）
		               = EN21 EN22 EN23 EN24 EN25 EN26 EN27 EN28

            g)  将运算后的结果（ENC BLOCK2）转换成16 个HEXDECIMAL：
            ENC BLOCK2 = EN21 EN22 EN23 EN24 EN25 EN26 EN27 EN28
            = EM211 EM212 EM221 EM222 EM231 EM232 EM241 EM242 ||
 			             EM251 EM252 EM261 EM262 EM271 EM272 EM281 EM282
            示例	：
            ENC RESULT= %H84, %H56, %HB1, %HCD, %H5A, %H3F, %H84, %H84
            转换成16 个HEXDECIMAL:
            “8456B1CD5A3F8484”
            h)  取前8个字节作为MAC值。
            取”8456B1CD”为MAC值。
            */
        /// <summary>
        /// 加密MAC
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="Key">工作密钥</param>
        /// <returns></returns>
        public static string MACEncrypt(byte[] buffer, string Key, int idx)
        {
            try
            {

                //-------------------------------------------------------------------------------------------------------------------------
                /* 文档中的描述
                 * 下面的内容是组成MAC ELEMEMENT BLOCK （MAB）的
                 * 文档中的描述
                 * a)  将欲发送给POS中心的消息中，从消息类型（MTI）到63域之间的部分构成MAC ELEMEMENT BLOCK （MAB）。
                 */
                //因为计算MAC时不需要报文头，所以去掉报文头的11字节
                byte[] macBuffer = new byte[buffer.Length - 11 - idx];
                Array.Copy(buffer, 11 + idx, macBuffer, 0, macBuffer.Length);
                //因为数据并不包含64域的内容，所以不需要去除最后64域的内容
                //计算后 macBuffer 的内容就是需要计算的内容

                //-------------------------------------------------------------------------------------------------------------------------

                /* 
                 * 文档中的描述
                 * b)  对MAB，按每8个字节做异或（不管信息中的字符格式），如果最后不满8个字节，则添加“0X00”。
                        示例	：
                        MAB = M1 M2 M3 M4
                        其中：	
                        M1 = MS11 MS12 MS13 MS14 MS15 MS16 MS17 MS18
                        M2 = MS21 MS22 MS23 MS24 MS25 MS26 MS27 MS28
                        M3 = MS31 MS32 MS33 MS34 MS35 MS36 MS37 MS38
                        M4 = MS41 MS42 MS43 MS44 MS45 MS46 MS47 MS48

                        按如下规则进行异或运算：
 			                        MS11 MS12 MS13 MS14 MS15 MS16 MS17 MS18
                        XOR）			MS21 MS22 MS23 MS24 MS25 MS26 MS27 MS28
                        ---------------------------------------------------
                        TEMP BLOCK1 =	TM11 TM12 TM13 TM14 TM15 TM16 TM17 TM18

                        然后，进行下一步的运算：
                        TM11 TM12 TM13 TM14 TM15 TM16 TM17 TM18
                        XOR）			MS31 MS32 MS33 MS34 MS35 MS36 MS37 MS38
                        ---------------------------------------------------
                        TEMP BLOCK2 =	TM21 TM22 TM23 TM24 TM25 TM26 TM27 TM28

                        再进行下一步的运算：
                        TM21 TM22 TM23 TM24 TM25 TM26 TM27 TM28
                        XOR）			MS41 MS42 MS43 MS44 MS45 MS46 MS47 MS48
                        ---------------------------------------------------
                        RESULT BLOCK =	TM31 TM32 TM33 TM34 TM35 TM36 TM37 TM38
                 */
                int index = 0;
                //取前两个需要异或的Block
                byte[] temp_block = new byte[8];
                byte[] sub_Mac = GetSubMac(macBuffer, index++ * 8);
                while (sub_Mac != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        //每8个字节异或
                        temp_block[i] = Convert.ToByte(sub_Mac[i] ^ temp_block[i]);
                    }
                    //获取下一个需要异或的8字节
                    //GetSubMac方法有具体的描述
                    sub_Mac = GetSubMac(macBuffer, index++ * 8);
                }

                //-------------------------------------------------------------------------------------------------------------------------
                /*文档中的描述
                 c)  将异或运算后的最后8个字节（RESULT BLOCK）转换成16 个HEXDECIMAL：
                    RESULT BLOCK = TM31 TM32 TM33 TM34 TM35 TM36 TM37 TM38
	                        = TM311 TM312 TM321 TM322 TM331 TM332 TM341 TM342 ||
		                      TM351 TM352 TM361 TM362 TM371 TM372 TM381 TM382
                 */
                string result_block = "";
                //因为后面暂时只用到了转换后的前8个字节，所以只处理一般长度 temp_block.Length / 2
                for (int i = 0; i < temp_block.Length / 2; i++)
                {
                    //将temp_block[i]中的值由10进制转化为16进制，如果不足2位左补0
                    result_block += SendConvertString(temp_block[i].ToString(), 10, 16).PadLeft(2, '0');
                }
                //转换为大写 关键
                result_block = result_block.ToUpper();

                //-------------------------------------------------------------------------------------------------------------------------
                /*文档中的描述
                 *  d)  取前8 个字节用MAK加密：
                    ENC BLOCK1 = eMAK（TM311 TM312 TM321 TM322 TM331 TM332 TM341 TM342）
			                    = EN11 EN12 EN13 EN14 EN15 EN16 EN17 EN18
                 */
                //存放加密后的内容
                byte[] enc_block = new byte[8];
                //加密 result_block的内容是之前计算好的HEXDECIMAL
                enc_block = LCDES.DesEncrypt(System.Text.Encoding.Default.GetBytes(result_block), Key);

                //-------------------------------------------------------------------------------------------------------------------------
                /* 文档中的描述
                    e)  将加密后的结果与后8 个字节异或：
                        EN11  EN12  EN13  EN14  EN15  EN16  EN17  EN18
                        XOR）     	TM351 TM352 TM361 TM362 TM371 TM372 TM381 TM382
                        ------------------------------------------------------------
                        TEMP BLOCK=	TE11  TE12  TE13  TE14  TE15  TE16  TE17  TE18
                 */
                //首先取后8个字节的内容 temp_block.Length / 2
                result_block = "";
                for (int i = temp_block.Length / 2; i < temp_block.Length; i++)
                {
                    //将temp_block[i]中的值由10进制转化为16进制，如果不足2位左补0
                    result_block += SendConvertString(temp_block[i].ToString(), 10, 16).PadLeft(2, '0');
                }
                //转换为大写 关键
                result_block = result_block.ToUpper();

                //将后8个字节的HEXDECIMAL转化为byte[]
                temp_block = System.Text.Encoding.Default.GetBytes(result_block);
                //将加密后的结果与后8 个字节异或
                for (int i = 0; i < 8; i++)
                {
                    temp_block[i] = Convert.ToByte(enc_block[i] ^ temp_block[i]);
                }

                //-------------------------------------------------------------------------------------------------------------------------
                /* 文档中的描述
                 f)  用异或的结果TEMP BLOCK 再进行一次单倍长密钥算法运算。
                        ENC BLOCK2 = eMAK（TE11 TE12 TE13 TE14 TE15 TE16 TE17 TE18）
		               = EN21 EN22 EN23 EN24 EN25 EN26 EN27 EN28
                 */
                //存放加密后的结果
                byte[] enc_block2 = new byte[8];
                enc_block2 = LCDES.DesEncrypt(temp_block, Key);
                //-------------------------------------------------------------------------------------------------------------------------

                /* 文档中的描述
                g)  将运算后的结果（ENC BLOCK2）转换成16 个HEXDECIMAL：
                        ENC BLOCK2 = EN21 EN22 EN23 EN24 EN25 EN26 EN27 EN28
                        = EM211 EM212 EM221 EM222 EM231 EM232 EM241 EM242 ||
 			             EM251 EM252 EM261 EM262 EM271 EM272 EM281 EM282
                h)  取前8个字节作为MAC值。
                 */
                result_block = "";
                //因为只取前8个字节作为MAC值，所以计算的长度是加密数组的一半 enc_block2.Length / 2
                for (int i = 0; i < enc_block2.Length / 2; i++)
                {
                    //将enc_block2[i]中的值由10进制转化为16进制，如果不足2位左补0
                    result_block += SendConvertString(enc_block2[i].ToString(), 10, 16).PadLeft(2, '0');
                }
                result_block = result_block.ToUpper();

                //-------------------------------------------------------------------------------------------------------------------------
                //到这里result_block的值就应该是MAC的值
                temp_block = System.Text.Encoding.Default.GetBytes(result_block);
                string ret = "";
                for (int i = 0; i < result_block.Length; i++)
                {
                    ret += SendConvertString(temp_block[i].ToString(), 10, 16).PadLeft(2, '0');
                }
                return ret;
            }
            catch (Exception)
            {

            }
            return "";
        }
        #endregion

        #region SendConvertString
        public static string SendConvertString(string value, int fromBase, int toBase)
        {
            int intValue = Convert.ToInt32(value, fromBase);
            return Convert.ToString(intValue, toBase);
        }
        #endregion

        #region 加密函数 用于MAC校验 DesEncrypt
        /// <summary>
        /// 加密函数 用于MAC校验
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static byte[] DesEncrypt(byte[] pToEncrypt, string sKey)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.None;
                ICryptoTransform myICT = des.CreateEncryptor(key, new byte[8]);

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, myICT, CryptoStreamMode.Write);

                cs.Write(pToEncrypt, 0, pToEncrypt.Length);
                cs.FlushFinalBlock();
                cs.Close();
                var buffer = ms.ToArray();
                ms.Close();
                return buffer;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region 按每8个字节获取内容 不足8字节后面补0x00 GetSubMac
        /// <summary>
        /// 按每8个字节获取内容 不足8字节后面补0x00
        /// </summary>
        /// <param name="macBuffer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static byte[] GetSubMac(byte[] macBuffer, int index)
        {
            byte[] buffer = new byte[8];

            if (macBuffer.Length <= index)
            {
                return null;
            }
            else if (macBuffer.Length - index < 8)
            {
                int len = macBuffer.Length - index;
                Array.Copy(macBuffer, index, buffer, 0, len);
                for (int i = 0; i < 8 - len; i++)
                {
                    buffer[len + i] = 0x00;
                }
            }
            else
            {
                Array.Copy(macBuffer, index, buffer, 0, 8);
            }
            return buffer;
        }
        #endregion

        #region MACDecrypt
        /// <summary>
        /// 解密MAC
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="Key"></param>
        /// <param name="MAC"></param>
        /// <returns></returns>
        public static bool MACDecrypt(byte[] buffer, string Key, string MAC)
        {
            //去除 mac8字节
            byte[] macBuffer = new byte[buffer.Length - 8];
            //拷贝需要校验MAC的内容
            Array.Copy(buffer, 0, macBuffer, 0, macBuffer.Length);
            //计算MAC的值
            string mac = LCDES.MACEncrypt(macBuffer, Key, 0);
            return MAC == mac ? true : false;
        }
        #endregion
    }
}