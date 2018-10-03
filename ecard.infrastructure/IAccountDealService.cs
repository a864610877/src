using System.ServiceModel;
using Ecard.Infrastructure;
using Ecard.Models;

namespace Ecard
{
    [ServiceContract]
    public interface IAccountDealService
    {
        /// <summary>
        /// �����ն˺ź��̻��ŷ�����Ӧ����Կ��Ϣ�����ն�ǩ��ʱ��ϵͳ��Ҫ�󱾷����ṩ�ն˶�Ӧ����Կ��8λ���������� ת��Ϊ 16 λ�ַ������� 3131313131313131, 
        /// ���ն˲����ڣ����ɷ��ؿգ��践��һ�������ڵ��ն˺ź�һ�������ڵ���Կ����
        /// </summary>
        /// <param name="posName">�ն˺�</param>
        /// <param name="shopName">�̻���</param>
        /// <param name="userName">ǩ��ʹ�õ��û�������Ϊ�գ���Ϊ����ǩ��������Pos��Ӧ�û�Ϊ�գ��������POS �����û���Ӧ��ϵ</param>
        /// <param name="password">ǩ��ʹ�õ��û�������Ϊ�գ���Ϊ����ǩ��</param>
        /// <returns></returns>
        [OperationContract]
        PosWithShop SignIn(string posName, string shopName, string userName, string password);
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse Pay(PayRequest request,bool IsWeb=false);
        /// <summary>
        /// Ԥ��Ȩ
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse PrePay(PayRequest request);
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse CancelPay(CancelPayRequest request, bool IsWeb = false);
        /// <summary>
        /// ���������ϴν����������ϣ���Ӧ���н���
        /// </summary>
        /// <param name="request">����</param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse Roolback(PayRequest_ request, bool IsWeb = false);
        /// <summary>
        /// ���Ԥ��Ȩ
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse DonePrePay(PayRequest request);
        /// <summary>
        ///����Ԥ��Ȩ���
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        AccountServiceResponse CancelDonePrePay(CancelPayRequest request);
        /// <summary>
        /// ��ѯ��Ա����Ϣ
        /// </summary>
        /// <param name="posName">�����ѯ���ն˺�</param>
        /// <param name="shopName">�����ѯ���̻���</param>
        /// <param name="accountName">��ѯ�Ŀ���</param>
        /// <param name="passwrod">��ѯ����</param>
        /// <param name="token">��Ա������</param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse Query(string posName, string shopName, string accountName, string passwrod, string token, string NewPassword = "", string OpenCode = "");
        /// <summary>
        /// ����Ԥ��Ȩ
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse CancelPrePay(CancelPayRequest request);
        /// <summary>
        /// ��ѯ�̻���Ϣ
        /// </summary>
        /// <param name="posName">�����ѯ���ն˺�</param>
        /// <param name="shopName">�����ѯ���̻���</param>
        /// <param name="shopTo">����ѯ���̻���</param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse QueryShop(string posName, string shopName, string shopTo);

        /// <summary>
        /// �����ֲ���������ֲ���ʹ���� PayRequest, ���Ǳ������жϿ�����
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse Integral(PayRequest request,bool IsWeb=false);
        /// <summary>
        /// �������ѣ��۳����֣�������
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse PayIntegral(PayRequest request);

        /// <summary>
        /// ��ֵ
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AccountServiceResponse Recharge(PayRequest request, bool IsWeb = false);
        
        /// <summary>
        /// �޸�����
        /// </summary>
        /// <param name="accountName">����</param>
        /// <param name="OldPwd">������</param>
        /// <param name="NewPwd">������</param>
        /// <returns></returns>
        AccountServiceResponse UpdatePwd(string accountName, string OldPwd, string NewPwd, string posName, string shopName, string UserToken);

        void InsertPosKey(PosKey item);
        void UpdatePosKey(PosKey item);
        PosKey GetPosKey(string ShopName, string PosName);
    }
}