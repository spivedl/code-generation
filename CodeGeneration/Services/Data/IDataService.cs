using System.Data;
using System.Data.SqlClient;

namespace CodeGeneration.Services.Data
{
    public interface IDataService
    {
        IDbConnection GetConnection(string connectionKey);
        DataSet QueryForDataSet(string connectionKey, string sql, bool logResults = false);
        DataSet QueryForDataSet(string connectionKey, string sql, SqlParameterCollection sqlParameterCollection, bool logResults = false);
        DataSet QueryForDataSet(string connectionKey, string sql, SqlParameter[] sqlParameterArray, bool logResults = false);
        int ExecuteNonQuery(string connectionKey, string sql);
        int ExecuteNonQuery(string connectionKey, string sql, SqlParameterCollection sqlParameterCollection);
        int ExecuteNonQuery(string connectionKey, string sql, SqlParameter[] sqlParameterArray);
        void LogDataSet(DataSet dataSet);
    }
}