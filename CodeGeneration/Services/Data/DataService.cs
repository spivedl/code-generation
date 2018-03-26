using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.Configuration;
using NLog;

namespace CodeGeneration.Services.Data
{
    public class DataService : IDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _configuration;

        public DataService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection GetConnection(string connectionKey)
        {
            Logger.Info($"Creating new SqlConnection for connection '{connectionKey}'.");
            return new SqlConnection(_configuration.GetConnectionString(connectionKey));
        }

        public DataSet QueryForDataSet(string connectionKey, string sql, bool logResults = false)
        {
            using (var connection = GetConnection(connectionKey))
            using (var sqlCommand = new SqlCommand(sql, (SqlConnection)connection))
            using (var dataAdapter = new SqlDataAdapter(sqlCommand))
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                if (logResults) LogDataSet(dataSet);

                return dataSet;
            }
        }

        public DataSet QueryForDataSet(string connectionKey, string sql, SqlParameterCollection sqlParameterCollection, bool logResults = false)
        {
            var sqlParameterArray = new SqlParameter[sqlParameterCollection.Count];

            for (var i = 0; i < sqlParameterCollection.Count; i++)
            {
                var param = sqlParameterCollection[i];
                sqlParameterArray[i] = new SqlParameter(param.ParameterName, param.Value);
            }

            return QueryForDataSet(connectionKey, sql, sqlParameterArray);
        }

        public DataSet QueryForDataSet(string connectionKey, string sql, SqlParameter[] sqlParameterArray, bool logResults = false)
        {
            using (var connection = GetConnection(connectionKey))
            using (var sqlCommand = new SqlCommand(sql, (SqlConnection)connection))
            using (var dataAdapter = new SqlDataAdapter(sqlCommand))
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                sqlCommand.Parameters.AddRange(sqlParameterArray);

                var dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                if (logResults) LogDataSet(dataSet);

                return dataSet;
            }
        }

        public int ExecuteNonQuery(string connectionKey, string sql)
        {
            using (var connection = GetConnection(connectionKey))
            using (var sqlCommand = new SqlCommand(sql, (SqlConnection)connection))
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                return sqlCommand.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(string connectionKey, string sql, SqlParameterCollection sqlParameterCollection)
        {
            var sqlParameterArray = new SqlParameter[sqlParameterCollection.Count];

            for (var i = 0; i < sqlParameterCollection.Count; i++)
            {
                var param = sqlParameterCollection[i];
                sqlParameterArray[i] = new SqlParameter(param.ParameterName, param.Value);
            }

            return ExecuteNonQuery(connectionKey, sql, sqlParameterArray);
        }

        public int ExecuteNonQuery(string connectionKey, string sql, SqlParameter[] sqlParameterArray)
        {
            using (var connection = GetConnection(connectionKey))
            using (var sqlCommand = new SqlCommand(sql, (SqlConnection)connection))
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                sqlCommand.Parameters.AddRange(sqlParameterArray);

                return sqlCommand.ExecuteNonQuery();
            }
        }

        public void LogDataSet(DataSet dataSet)
        {
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                Logger.Info("The data set was null or did not contain any data.");
                return;
            }

            // each table in the dataset represents a different result set from the stored procedure
            // by looping over each table, then each row, then each column we can effectively catch 
            // any arbitrary combination of result sets, rows, and columns from any stored procedure
            // and then print that out to the application log file
            var sbLine = new StringBuilder();
            for (var i = 0; i < dataSet.Tables.Count; i++)
            {
                var table = dataSet.Tables[i];
                if (table.Rows.Count <= 0) continue;

                Logger.Info($"Records in data table [{i}]...");

                #region Print Column Headers

                foreach (DataColumn column in table.Columns)
                {
                    sbLine.Append($"{column.ColumnName.PadRight(25)}");
                }

                Logger.Info(sbLine.ToString());
                sbLine.Clear();

                #endregion
                #region Print Rows
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        sbLine.Append($"{row[column.ColumnName].ToString().PadRight(25)}");
                    }

                    Logger.Info(sbLine.ToString());
                    sbLine.Clear();
                }
                #endregion
            }
        }
    }
}