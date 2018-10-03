using System;
using System.IO;
using System.Linq;
using System.Net;
using Ecard;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Moonlit.Data;
using log4net;
//using MWW.Net;
using PI8583.Protocal;
using System.Text;

//namespace PI8583.Network
//{
//    public class I8583ReactorHandler<TFactory> : ReactorHandler<IPEndPoint>
//        where TFactory : IDealServiceFactory
//    {
//        private ILog _log = log4net.LogManager.GetLogger(typeof(I8583ReactorHandler<TFactory>));
//        private MemoryStream _stream = new MemoryStream();

//        public override void OnConnected()
//        {
//            _log.Debug("connected from " + Handler.RemoteEndPoint.ToString());
//        }
//        public string GetCurrentPath(string dir)
//        {
//            return Path.GetDirectoryName(typeof(I8583ReactorHandler<TFactory>).Assembly.Location) + "\\" + dir;
//        }
//        public override void OnReceived(byte[] data)
//        {
//            string aa = "";// string.Join("-", data.Select(x => x.ToString("x").PadLeft(2, '0')));
//            for (int i = 0; i < data.Length; i++)
//            {
//                aa += data[i].ToString()+"-";
//            }
//                //string ss =System.Text.Encoding.Unicode.GetString(data);
//                //_log.Debug("zifc: " + ss);
//                //byte[] cc = System.Text.Encoding.Default.GetBytes(ss);
//                if (Convert.ToDateTime("2092-10-30") < DateTime.Now)
//                {
//                    string str = "已经超出使用期限";
//                    Send(Encoding.Default.GetBytes(str));
//                    File.WriteAllText(string.Format("{0}\\{1}.{2}.txt", GetCurrentPath("packs"), DateTime.Now.ToString(), "error"), str);
//                }
//            if (!Directory.Exists(GetCurrentPath("packs")))
//            {
//                Directory.CreateDirectory(GetCurrentPath("packs"));
//            } 
//            IResponse rsp = null;
//            try
//            {
                
//                _log.Debug("received from " + Handler.RemoteEndPoint.ToString());
//                _log.Debug("received data " + string.Join("-", data.Select(x => x.ToString("x").PadLeft(2, '0'))));
//                _stream.Seek(0, SeekOrigin.End);
//                _stream.Write(data, 0, data.Length);
//                _stream.Seek(0, SeekOrigin.Begin);
//                if (_stream.Length > 1024 * 1024 * 2)
//                {
//                    this.Handler.Disconnect(false);
//                }
//                while (_stream.Length > 2)
//                {
//                    Database database = new Database("ecard");

//                    var bytes = PI8583Factory.Read(_stream);
//                    if (bytes == null)
//                    {
//                        break;
//                    };
//                    var position = (int)_stream.Position;
//                    _stream = new MemoryStream();
//                    byte[] remain = _stream.ToArray().Skip(position).ToArray();
//                    _stream.Write(remain, 0, remain.Length);
//                    using (var instance = database.OpenInstance())
//                    {
//                        instance.BeginTransaction();
//                        string name = "AA" + Guid.NewGuid().ToString("N");
//                        var accountDealService = CreateAccountService(instance);
//                        I8638Context context = new I8638Context(accountDealService);
//                        IRequest req = PI8583Factory.Create(bytes, context);
//                        if (req != null)
//                        {
//                            //File.WriteAllBytes(string.Format("{0}\\{1}.{2}.txt", GetCurrentPath("packs"), name, req.GetType().FullName), bytes);
//                            //rsp = req.GetResponse();
//                            //byte[] buffer = rsp.GetData();

//                            //if (rsp.Result == ResponseCode.Success)
//                            //    instance.Commit();

//                            //_log.Debug("send data: " + (buffer ?? new byte[0]).Length);
//                            //Send(buffer);
//                            //File.WriteAllBytes(string.Format("{0}\\{1}.{2}.txt", GetCurrentPath("packs"), name, rsp.GetType().FullName), buffer);

//                            //StreamReader sr = new StreamReader(@"D:\My Office\张飞牛肉\新建文件夹\AA00a1542da2f144dc9482c5035082405d.PI8583.DealRequest---.txt");
//                            //string s = "";
//                            //string txt = "";
//                            //while ((s = sr.ReadLine()) != null)
//                            //{
//                            //    //上面一行一行读。然后在里面就看你自己怎么处理了。下面是假设。
//                            //    //if (s == "2001")
//                            //    //{
//                            //    txt += s;

//                            //    //}
//                            //}
//                            ////txt = txt.Replace("msgtype--", "");
//                            ////txt = txt.Replace("--", "");
//                            //byte[] ss = Encoding.Default.GetBytes(txt);
//                            //PI8385Extensions.Display(ss);
//                           // PI8385Extensions.Display(@"D:\My Office\张飞牛肉\新建文件夹\packs\AA00a1542da2f144dc9482c5035082405d.PI8583.DealRequest.txt");
//                            I8583 packreq = new I8583(I8583.GetMessageType(bytes));
//                            packreq.UnPack8583(bytes);
//                            string str = PI8385Extensions.Display(packreq);
//                            File.WriteAllText(string.Format("{0}\\{1}.{2}.txt", GetCurrentPath("packs"), name, req.GetType().FullName), str);
//                            rsp = req.GetResponse();
//                            byte[] buffer = rsp.GetData();

//                            if (rsp.Result == ResponseCode.Success)
//                                instance.Commit();

//                            _log.Debug("send data: " + string.Join("-", buffer.Select(x => x.ToString("x").PadLeft(2, '0'))));
//                            Send(buffer);
//                            var temp = buffer.Skip(2).ToArray();
//                            I8583 packrsp = new I8583(I8583.GetMessageType(temp));
//                            packrsp.UnPack8583(temp);
//                            string strrsp = PI8385Extensions.Display(packrsp);
//                            File.WriteAllText(string.Format("{0}\\{1}.{2}.txt", GetCurrentPath("packs"), name, rsp.GetType().FullName), strrsp);
//                        }
//                        else
//                        {
//                            break;
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                try
//                {
//                    Clear(data, ex, rsp);
//                }
//                catch (Exception)
//                {
//                }
//                _log.Error("error ", ex);
//            }
//        }

//        private IAccountDealService CreateAccountService(DatabaseInstance instance)
//        {
//            var factory = (IDealServiceFactory)Activator.CreateInstance(typeof (TFactory));
//            return factory.CreateService(instance);
//        }

//        private void Clear(byte[] data, Exception ex, IResponse rsp)
//        {
//            if (rsp != null)
//            {
//                Send(rsp.GetError());
//            }
//            string guid = Guid.NewGuid().ToString("N");
//            _log.Debug("fail to process data(" + guid + "):" + ex.ToString());
//            if (!Directory.Exists(GetCurrentPath("errorPacks")))
//            {
//                Directory.CreateDirectory(GetCurrentPath("errorPacks"));
//            }
//            File.WriteAllBytes(GetCurrentPath("errorPacks") + "\\" + guid + ".data", data);
//            if (_stream != null)
//                File.WriteAllBytes(GetCurrentPath("errorPacks") + "\\" + guid + ".stream", _stream.ToArray());
//        }

//        public override void OnDisconnected()
//        {
//            _log.Debug("disconnected from " + Handler.RemoteEndPoint.ToString());
//        }
//    }

//}

namespace PI8583.Network
{
    public class I8583ReactorHandler : ReactorHandler
    {
        private ILog _log = LogManager.GetLogger(typeof(I8583ReactorHandler));
        private MemoryStream _stream = new MemoryStream();

        public override void OnConnected()
        {
            _log.Debug("connected from " + Handler.RemoteEndPoint.ToString());
        }
        public string GetCurrentPath(string dir)
        {
            return Path.GetDirectoryName(typeof(I8583ReactorHandler).Assembly.Location) + "\\" + dir;
        }
        public override void OnReceived(byte[] data)
        {
            if (Convert.ToDateTime("2093-06-10") < DateTime.Now)
            {
                string str = "已经超出使用期限";
                Send(Encoding.Default.GetBytes(str));
                File.WriteAllText(string.Format("{0}\\{1}.{2}.txt", GetCurrentPath("packs"), DateTime.Now.ToString(), "error"), str);
            }
            IResponse rsp = null;
            try
            {
                _log.Debug("received from " + Handler.RemoteEndPoint.ToString());
                _log.Debug("received data " + string.Join("-", data.Select(x => x.ToString("x").PadLeft(2, '0'))));
                _stream.Seek(0, SeekOrigin.End);
                _stream.Write(data, 0, data.Length);
                _stream.Seek(0, SeekOrigin.Begin);
                if (_stream.Length > 1024 * 1024 * 2)
                {
                    this.Handler.Disconnect(false);
                }
                while (_stream.Length > 2)
                {
                    Database database = new Database("ecard");

                    var bytes = PI8583Factory.Read(_stream);
                    if (bytes != null)
                    {
                        var position = (int)_stream.Position;
                        _stream = new MemoryStream();
                        byte[] remain = _stream.ToArray().Skip(position).ToArray();
                        _stream.Write(remain, 0, remain.Length);
                    }
                    using (var instance = database.OpenInstance())
                    {
                        instance.BeginTransaction();

                        CachedSqlAccountDealDal dal = new CachedSqlAccountDealDal(instance);
                        SmsDealTracker dealTracker = new SmsDealTracker(dal, new SmsHelper(new SqlSmsService(instance)), dal.GetSite());
                        SqlOrderService sqlOrderService=new SqlOrderService(instance);
                        IPosKeyService PosKeyService = new SqlPosKeyService(instance);
                        IAccountDealService accountDealService = new AccountDealService(dal, dealTracker, sqlOrderService, PosKeyService);
                        
                        I8638Context context = new I8638Context(accountDealService);
                        IRequest req = PI8583Factory.Create(bytes, context);

                        if (req != null)
                        {
                            rsp = req.GetResponse();
                            byte[] buffer = rsp.GetData();

                            if (rsp.Result == ResponseCode.Success)
                                instance.Commit();

                            _log.Debug("send data: " + (buffer ?? new byte[0]).Length);
                            _log.Debug("send data " + string.Join("-", buffer.Select(x => x.ToString("x").PadLeft(2, '0'))));
                            Send(buffer);
                            File.WriteAllBytes("send.dat", buffer);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Clear(data, ex, rsp);
                }
                catch (Exception)
                {
                }
                _log.Error("error ", ex);
            }
        }
        //private IAccountDealService CreateAccountService(DatabaseInstance instance)
        //{
        //    var factory = (IDealServiceFactory)Activator.CreateInstance(typeof(TFactory));
        //    return factory.CreateService(instance);
        //}
        private void Clear(byte[] data, Exception ex, IResponse rsp)
        {
            if (rsp != null)
            {
                Send(rsp.GetError());
            }
            string guid = Guid.NewGuid().ToString("N");
            _log.Debug("fail to process data(" + guid + "):" + ex.ToString());
            if (!Directory.Exists(GetCurrentPath("errorPacks")))
            {
                Directory.CreateDirectory(GetCurrentPath("errorPacks"));
            }
            File.WriteAllBytes(GetCurrentPath("errorPacks") + "\\" + guid + ".data", data);
            if (_stream != null)
                File.WriteAllBytes(GetCurrentPath("errorPacks") + "\\" + guid + ".stream", _stream.ToArray());
        }

        protected override void OnDisconnected()
        {
            _log.Debug("disconnected from " + Handler.RemoteEndPoint.ToString());
        }
    }

}
