using System;
using Ecard.Models;

namespace Ecard.Services
{

    public interface ICashDealLogSummaryService
    {
        QueryObject<CashDealLogSummary> Query(CashDealLogRequest request); 
    }
     
    public interface ICashDealLogService
    {
        QueryObject<CashDealLog> Query(CashDealLogRequest request);
        void Create(CashDealLog item);
        CashDealLog GetById(int id);
        void Update(CashDealLog item);
        void Delete(CashDealLog item);
        decimal GetSummary(int ownerId);
    }

    public class CashDealLogRequest
    {
        public DateTime? SubmitTimeMax { get; set; }

        public DateTime? SubmitTimeMin { get; set; }

        public int? OwnerId { get; set; }
    }

}