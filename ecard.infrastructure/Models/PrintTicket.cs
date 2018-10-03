using System;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    /// <summary>
    /// This object represents the properties and methods of a Log.
    /// </summary>
    public class PrintTicket
    {
        public PrintTicket(int logType, string serialNo, string content, Account account)
        {
            LogType = logType;
            Content = content;
            SubmitTime = DateTime.Now;
            this.AccountId = account.AccountId;
            this.AccountName = account.Name;
            SerialNo = serialNo;
            PrintCount = 1;
        }

        public PrintTicket()
        {

        }
        [Key]
        public int PrintTicketId { get; set; }
        public int AccountId { get; set; }
        public string SerialNo { get; set; }
        public string AccountName { get; set; }
        public string Content { get; set; }
        public DateTime SubmitTime { get; set; }
        [Bounded(typeof(LogTypes))]
        public int LogType { get; set; }
        public int PrintCount { get; set; }
    }
}