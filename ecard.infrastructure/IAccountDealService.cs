using System.ServiceModel;
using Ecard.Infrastructure;
using Ecard.Models;

namespace Ecard
{
    [ServiceContract]
    public interface IAccountDealService
    {
        /// <summary>
        /// 根据终端号和商户号返回相应的密钥信息，在终端签到时，系统会要求本服务提供终端对应的密钥，8位二进制数组 转换为 16 位字符串，如 3131313131313131, 
        /// 若终端不存在，不可返回空，需返回一个不存在的终端号和一个不存在的密钥即可
        /// </summary>
        /// <param name="posName">终端号</param>
        /// <param name="shopName">商户号</param>
        /// <param name="userName">签到使用的用户名，如为空，则为本地签到，更新Pos对应用户为空，否则更新POS 机与用户对应关系</param>
        /// <param name="password">签到使用的用户名，如为空，则为本地签到</param>
        /// <returns></returns>
        [OperationContract]
        PosWithShop SignIn(string posName, string shopName, string userName, string password);
        /// <summary>
        /// 交易
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse Pay(PayRequest request,bool IsWeb=false);
        /// <summary>
        /// 预授权
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse PrePay(PayRequest request);
        /// <summary>
        /// 撤销交易
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse CancelPay(CancelPayRequest request, bool IsWeb = false);
        /// <summary>
        /// 冲正，即上次交易完整作废，对应所有交易
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse Roolback(PayRequest_ request, bool IsWeb = false);
        /// <summary>
        /// 完成预授权
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse DonePrePay(PayRequest request);
        /// <summary>
        ///撤销预授权完成
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        AccountServiceResponse CancelDonePrePay(CancelPayRequest request);
        /// <summary>
        /// 查询会员卡信息
        /// </summary>
        /// <param name="posName">发起查询方终端号</param>
        /// <param name="shopName">发起查询方商户号</param>
        /// <param name="accountName">查询的卡号</param>
        /// <param name="passwrod">查询密码</param>
        /// <param name="token">会员卡暗码</param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse Query(string posName, string shopName, string accountName, string passwrod, string token, string NewPassword = "", string OpenCode = "");
        /// <summary>
        /// 撤销预授权
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse CancelPrePay(CancelPayRequest request);
        /// <summary>
        /// 查询商户信息
        /// </summary>
        /// <param name="posName">发起查询方终端号</param>
        /// <param name="shopName">发起查询方商户号</param>
        /// <param name="shopTo">被查询的商户号</param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse QueryShop(string posName, string shopName, string shopTo);

        /// <summary>
        /// 单积分操作，单向分操作使用了 PayRequest, 但是本身并不判断卡密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse Integral(PayRequest request,bool IsWeb=false);
        /// <summary>
        /// 积分消费，扣除积分，再消费
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse PayIntegral(PayRequest request);

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse Recharge(PayRequest request, bool IsWeb = false);
        
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="accountName">卡号</param>
        /// <param name="OldPwd">旧密码</param>
        /// <param name="NewPwd">新密码</param>
        /// <returns></returns>
        AccountServiceResponse UpdatePwd(string accountName, string OldPwd, string NewPwd, string posName, string shopName, string UserToken);

        void InsertPosKey(PosKey item);
        void UpdatePosKey(PosKey item);
        PosKey GetPosKey(string ShopName, string PosName);
    }
}