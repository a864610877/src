using System;
using System.ComponentModel.DataAnnotations;
using Moonlit.Runtime.Serialization;

namespace Ecard.Models
{
    public class Task
    {
        public Task()
        {
            
        }

        public Task(object command, int creatorId )
        {
            var commandType = command.GetType();
            this.CommandTypeName = GetCommandType(commandType);
            this.CommandParameter = command.SerializeAsDataContractJson();
            CreatorId = creatorId;
            SubmitTime = DateTime.Now;
            State = TaskStates.Normal;
        }

        public static string GetCommandType(Type commandType)
        {
            return commandType.FullName + ", " + commandType.Assembly.GetName().Name;
        }

        [Key]
        public int TaskId { get; set; }
        public string CommandTypeName { get; set; }
        public string CommandParameter { get; set; }
        public DateTime SubmitTime { get; set; }
        public DateTime? ExecuteTime { get; set; }
        public string Error { get; set; }
        public int EditorId { get; set; }
        public int CreatorId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        [Bounded(typeof(TaskStates))]
        public int State { get; set; }
        public int RecordVersion { get; set; }
    }
}