using PostMsg_Net;
using PostMsg_Net.common;
using System;
using System.Collections.Generic;
using com.xuanwu.msggate.common.protobuf;
using Ecard.XWKJSms;
namespace Ecard.XWKJ
{
    class SmsBase
    {
        PostMsg postMsg;
        /// <summary>
        /// 新发送接口
        /// </summary>
        private string struuid = "";
        private bool SetUser(string userName, string pwd)
        {
            try
            {
                postMsg.SetUser(userName, pwd);
                return true;
            }
            catch (Exception ex)
            {
                //throw new Exception("短信账户设置异常", ex);
                return false;
            }
        }
        /// <summary>
        /// 上行网关设置
        /// </summary>
        /// <param name="CmIp"></param>
        /// <param name="cmPort"></param>
        /// <returns></returns>
        private bool SetMOAddress(string cmIp, int cmPort)
        {
            try
            {
                postMsg.SetMOAddress(cmIp, cmPort, LinkType.SHORTLINK);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 下行网关设置
        /// </summary>
        /// <param name="dlIp"></param>
        /// <param name="dlPort"></param>
        /// <returns></returns>
        private bool SetGateWay(string dlIp, int dlPort)
        {
            try
            {
                postMsg.SetGateWay(dlIp, dlPort, LinkType.SHORTLINK);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Init()
        {
            postMsg = new PostMsg();
            return SetUser(Config.UserName, Config.Pwd) && SetMOAddress(Config.CmIp, Config.CmPort) && SetGateWay(Config.DlIp, Config.DlPort);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pms"></param>
        /// <param name="rm"></param>
        /// <returns>是否发送成功</returns>
        public bool Send(List<SmsPhoneMsg> pms,out SmsResponseMsg rm,bool isMass)
        {
            #region
            try
            {
                DateTime now = DateTime.Now;
                Guid PostGuid = Guid.Empty;
                PostGuid = Guid.NewGuid();
                MTPack pack = new MTPack();

                MessageData[] Messagedatas = null;
                if (isMass)
                {
                    //群发
                    if (pms.Count > 0)
                    {
                        Messagedatas = new MessageData[pms.Count];
                        for (int i = 0; i < pms.Count; i++)
                        {

                            Messagedatas[i] = new MessageData();
                            Messagedatas[i].Content = pms[i].Content;
                            Messagedatas[i].Phone = pms[i].Phone.Trim();
                            Messagedatas[i].vipFlag = false;
                            Messagedatas[i].customMsgID = Guid.NewGuid().ToString();
                        }
                        pack.sendType = (int)PostMsg_Net.SendType.MASS;//具体是哪种发送模式具体分析下
                    }
                }
                else
                {
                    //组发
                    Messagedatas = new MessageData[pms.Count];
                    for (int i = 0; i < pms.Count; i++)
                    {
                        Messagedatas[i] = new MessageData();
                        Messagedatas[i].Content = pms[i].Content.Trim();
                        Messagedatas[i].Phone = pms[i].Phone.Trim();
                        Messagedatas[i].vipFlag = false;
                        Messagedatas[i].customMsgID = Guid.NewGuid().ToString();
                    }
                    pack.sendType = (int)PostMsg_Net.SendType.GROUP;//具体是哪种发送模式具体分析下
                }
                pack.msgs = Messagedatas;
                pack.msgType = (int)PostMsg_Net.MsgType.SMS;
                pack.batchID = PostGuid;

                #region 可选设置
                //DateTime ttt;
                //bool flag1, flag2;
                //可选设置
                //if (!string.IsNullOrEmpty(txtSubid.Text.Trim()))
                //    pack.customNum = txtSubid.Text.Trim();//扩展号
                //if (!string.IsNullOrEmpty(textBox4.Text))
                //{
                //    flag1 = DateTime.TryParse(textBox4.Text, out ttt);
                //    if (!flag1)
                //    {
                //        txtResult.AppendText("发送批次号:00000000-0000-0000-0000-000000000000\r\n返回处理结果状态码:-2\r\n结果信息：参数无效");
                //        return;
                //    }
                //}
                //if (!string.IsNullOrEmpty(textBox8.Text))
                //{
                //    flag2 = DateTime.TryParse(textBox8.Text, out ttt);
                //    if (!flag2)
                //    {
                //        txtResult.AppendText("发送批次号:00000000-0000-0000-0000-000000000000\r\n返回处理结果状态码:-2\r\n结果信息：参数无效");
                //        return;
                //    }
                //}
                //if (!string.IsNullOrEmpty(textBox4.Text))
                //    pack.scheduleTime = DateAndTicks.GetTicksFromDate(DateTime.Parse(textBox4.Text));
                //if (!string.IsNullOrEmpty(textBox8.Text))
                //    pack.deadline = DateAndTicks.GetTicksFromDate(DateTime.Parse(textBox8.Text));
                //if (!string.IsNullOrEmpty(textBox12.Text))
                //    pack.batchName = textBox12.Text;
                //if (!string.IsNullOrEmpty(comboBox1.Text))
                //{
                //    pack.distinctFlag = comboBox1.Text == "是" ? true : false;
                //}
                #endregion
                

                //业务类型（0-20之间）
                pack.bizType = 0;
                GsmsResponse getresponse = postMsg.Post(postMsg.GetAccount(), pack);
                TimeSpan t = DateTime.Now.Subtract(now);

                int tt = Convert.ToInt32(t.Seconds);

                rm = new SmsResponseMsg();
                if (getresponse != null)
                {
                    rm.Uuid = getresponse.uuid;
                    rm.Result = getresponse.result;
                    rm.Message = getresponse.message;
                    rm.Attributes = getresponse.attributes;
                    rm.UseMillisecond = tt;
                    struuid = getresponse.uuid.ToString();
                    if (getresponse.result!=(int)Result.SUCCESS)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                rm = new SmsResponseMsg();
                rm.Result = -1;
                rm.Message = ex.Message;
                return false;
            }
            #endregion
        }
    }
}
