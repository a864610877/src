using System;
using Ecard.Models;

namespace Ecard.Mvc.Models.Tasks
{
    public class ListTask
    {
        private readonly Task _innerObject;

        [NoRender]
        public Task InnerObject
        {
            get { return _innerObject; }
        }

        public ListTask()
        {
            _innerObject = new Task();
        }

        public decimal Amount
        {
            get { return InnerObject.Amount; }
        }

        public string AccountName { get; set; }
        public ListTask(Task innerObject)
        {
            _innerObject = innerObject;
        }

        public DateTime SubmitTime
        {
            get { return InnerObject.SubmitTime; }
            set { InnerObject.SubmitTime = value; }
        }
        public DateTime? ExecuteTime
        {
            get { return InnerObject.ExecuteTime; }
            set { InnerObject.ExecuteTime = value; }
        }
        public string Error
        {
            get { return InnerObject.Error; }
            set { InnerObject.Error = value; }
        }

        public string CreatorUserName { get; set; }
        public string EditorUserName { get; set; }
        public string CommandName { get; set; }
        //public object DescriptionObject { get; set; }
        [NoRender]
        public int TaskId
        {
            get { return InnerObject.TaskId; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }
    }
}