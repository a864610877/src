using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecard.Models
{
    public class PointRebate : IAccountDependency, IKeySetter
    {
        private int _dependencyType;

        private string _weekDays;

        private string _days;

        private string _accountLevels;

        [Key]
        public int PointRebateId { get; set; }
        public string DisplayName { get; set; }
        public int Point { get; set; }
        public decimal Amount { get; set; }
        [Bounded(typeof(PointRebateStates))]
        public int State { get; set; }

        public int DependencyType
        {
            get { return _dependencyType; }
            set { _dependencyType = value; }
        }

        public string WeekDays
        {
            get { return _weekDays; }
            set { _weekDays = value; }
        }

        public string Days
        {
            get { return _days; }
            set { _days = value; }
        }

        public string AccountLevels
        {
            get { return _accountLevels; }
            set { _accountLevels = value; }
        }

        int IKeySetter.Id
        {
            get { return PointRebateId; }
            set { PointRebateId = value; }
        }
    }
}
