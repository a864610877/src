using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.Shops
{
    public class WindowTicketingView
    {
        private Bounded _babySexBounded;
        private Bounded _payTypeBounded;
        private Bounded _admissionTicketBounded;
        public Bounded AdmissionTicket
        {
            get
            {
                if (_admissionTicketBounded == null)
                {
                    _admissionTicketBounded = Bounded.CreateEmpty("AdmissionTicketId", 0);
                }
                return _admissionTicketBounded;
            }
            set { _admissionTicketBounded = value; }
        }

        public decimal amount { get; set; }

        public Bounded PayType
        {
            get
            {
                if (_payTypeBounded == null)
                {
                    _payTypeBounded = Bounded.CreateEmpty("AdmissionTicketId", 0);
                }
                return _admissionTicketBounded;
            }
            set { _payTypeBounded = value; }
        }

        public string DisplayName { get; set; }
        public string Mobile { get; set; }
        public string BabyName { get; set; }
        public Bounded BabySex
        {
            get
            {
                if (_payTypeBounded == null)
                {
                    _payTypeBounded = Bounded.CreateEmpty("AdmissionTicketId", 0);
                }
                return _admissionTicketBounded;
            }
            set { _payTypeBounded = value; }
        }
    }
}
