using Ecard.Infrastructure;
using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Services
{
    public interface IHandRingPrintService
    {
        void Insert(HandRingPrint item);
        void Update(HandRingPrint item);
        HandRingPrint GetById(int id);
        DataTables<HandRingPrint> GetList(HandRingPrintRequest request);
    }

    public class HandRingPrintRequest
    {
        public string code { get; set; }
        public string babyName { get; set; }
        public string mobile { get; set; }
        public int? state { get; set; }
        public int? ticketType { get; set; }
        public int? shopId { get; set; }
        public DateTime? Bdate { get; set; }
        public DateTime? Edate { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}
