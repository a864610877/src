using Ecard.Models;

namespace Ecard.Services
{
    public interface IPrePayService
    {
        QueryObject<PrePay> Query(PrePayRequest request);
        void Create(PrePay item);
        PrePay GetById(int id);
        void Update(PrePay item);
        void Delete(PrePay item);
    }
}