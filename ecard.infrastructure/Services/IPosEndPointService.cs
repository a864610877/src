using System.Linq;
using Ecard.Models;
using Moonlit;
using Moonlit.Data;
using Ecard.Infrastructure;
using System.Data.SqlClient;

namespace Ecard.Services
{
    public interface IPosEndPointService
    {
        QueryObject<PosEndPoint> Query(PosEndPointRequest request);
        void Create(PosEndPoint item);
        PosEndPoint GetById(int id);
        void Update(PosEndPoint item);
        void Delete(PosEndPoint item);
        PosEndPoint GetByName(string posName);
        void UpdateCurrenUserId(int posEndPointId, int userId);
        PosEndPoint GetByCurrentUserId(int userId);
        DataTables<PosEndPoint> New_Query(PosEndPointRequest request);
    }

    public class PosEndPointRequest
    {
        private string _name;
        private string _nameWith;
        public int? ShopId { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string Name
        {
            get { return _name.NullIfEmpty(); }
            set { _name = value; }
        }

        public string NameWith
        {
            get { return _nameWith.NullIfEmpty(); }
            set { _nameWith = value; }
        }

        public int? State { get; set; }
    }

    public class SqlPosEndPointService : IPosEndPointService
    {
        private const string TableName = "PosEndPoints";
        private readonly DatabaseInstance _databaseInstance;

        public SqlPosEndPointService(DatabaseInstance databaseInstance)
        {
            _databaseInstance = databaseInstance;
        }

        #region IPosEndPointService Members

        public QueryObject<PosEndPoint> Query(PosEndPointRequest request)
        {
            return new QueryObject<PosEndPoint>(_databaseInstance, "PosEndPoint.query", request);
        }

        public DataTables<PosEndPoint> New_Query(PosEndPointRequest request)
        {
            SqlParameter[] param = { 
                                   new SqlParameter("@Name",request.Name),
                                   new SqlParameter("@NameWith",request.NameWith), 
                                   new SqlParameter("@State",request.State),
                                   new SqlParameter("@pageIndex",request.PageIndex),
                                   new SqlParameter("@pageSize",request.PageSize),
                                   new SqlParameter("@ShopId",request.ShopId) 
                                   };
            StoreProcedure sp = new StoreProcedure("P_getPosEndPoints", param);
            return _databaseInstance.GetTables<PosEndPoint>(sp);
        }
        public void Create(PosEndPoint item)
        {
            item.PosEndPointId = _databaseInstance.Insert(item, TableName);
        }

        public PosEndPoint GetById(int id)
        {
            return _databaseInstance.GetById<PosEndPoint>(TableName, id);
        }

        public void Update(PosEndPoint item)
        {
            _databaseInstance.Update(item, TableName);
        }

        public void Delete(PosEndPoint item)
        {
            _databaseInstance.Delete(item, TableName);
        }

        public PosEndPoint GetByName(string posName)
        {
            var sql = "select * from posEndPoints where name = @name";
            return _databaseInstance.Query<PosEndPoint>(sql, new { name = posName }).FirstOrDefault();
        }

        public void UpdateCurrenUserId(int posEndPointId, int userId)
        {
            var sql = "update posEndPoints set currentUserId = @userId, recordversion = recordversion +1 where posEndPointId = @posEndPointId ";
            _databaseInstance.ExecuteNonQuery(sql, new { posEndPointId = posEndPointId, userId = userId });
        }

        public PosEndPoint GetByCurrentUserId(int userId)
        {
            var sql = "select * from posEndPoints where currentUserId = @userId";
            return _databaseInstance.Query<PosEndPoint>(sql, new { userId = userId }).FirstOrDefault();
        }

        #endregion
    }
}