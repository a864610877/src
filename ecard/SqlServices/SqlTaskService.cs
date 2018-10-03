using Ecard.Models;

namespace Ecard.Services
{
    public class SqlTaskService : ITaskService
    {
        private readonly DatabaseInstance _databaseInstance;
        private const string TableName = "Tasks";

        public SqlTaskService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        public QueryObject<Task> Query(TaskRequest request)
        {
            var sql = "Select * from " + TableName + 
                " where ( commandTypeName = @commandTypeName ) " +
                      "and (@State is null or State = @state) " +
                      "and (@accountId is null or accountid = @accountId)";
            return new QueryObject<Task>(_databaseInstance,sql, request);
        }

        public void Create(Task item)
        {
            item.TaskId = _databaseInstance.Insert(item, TableName);
        }

        public Task GetById(int id)
        {
            return _databaseInstance.GetById<Task>(TableName, id);
        }

        public void Update(Task item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(Task item)
        {
            _databaseInstance.Delete(item, TableName);
        }
    }
}