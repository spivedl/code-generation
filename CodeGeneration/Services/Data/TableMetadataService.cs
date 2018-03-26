using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CodeGeneration.Models.Metadata.Sql;
using NLog;

namespace CodeGeneration.Services.Data
{
    public class TableMetadataService : ITableMetadataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDataService _dataService;
        private readonly IDictionary<string, ISet<TableMetadata>> _cache;

        public TableMetadataService(IDataService dataService)
        {
            _dataService = dataService;
            _cache = new Dictionary<string, ISet<TableMetadata>>(StringComparer.OrdinalIgnoreCase);
        }

        public ISet<TableMetadata> GetTableMetadata(string connectionKey, string targetDatabase, string targetSchema)
        {
            var key = BuildDictionaryKey(targetDatabase, targetSchema);
            Logger.Info($"Getting table definitions for {key}.");

            if (_cache.ContainsKey(key))
            {
                Logger.Info($"[CACHE HIT] Table definitions for {key} already in cache. Will not retrieve from database.");
                return _cache[key];
            }

            try
            {
                Logger.Info($"[CACHE MISS] Table definitions for {key} NOT in cache. Will retrieve information from database and add to cache.");

                var tableMetadataSet = InitializeTableMetadata(connectionKey, targetDatabase, targetSchema);
                tableMetadataSet = UpdateTableRelationships(connectionKey, targetDatabase, targetSchema, tableMetadataSet);

                _cache.Add(key, tableMetadataSet);

                return tableMetadataSet;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error attempting to retreive table definitions or relationships for '[{targetDatabase}].[{targetSchema}]'.");
                throw;
            }
        }

        private ISet<TableMetadata> InitializeTableMetadata(string connectionKey, string targetDatabase, string targetSchema)
        {
            Logger.Info($"Retrieving table definitions from {targetDatabase} for schema '{targetSchema}'.");

            const string sql = @"
            SELECT  t.TABLE_CATALOG
		            , t.TABLE_SCHEMA
		            , t.TABLE_NAME
		            , c.COLUMN_NAME
		            , c.ORDINAL_POSITION
		            , c.COLUMN_DEFAULT
		            , c.IS_NULLABLE
		            , c.DATA_TYPE
                    , c.CHARACTER_MAXIMUM_LENGTH
		            , CONVERT(INT, c.NUMERIC_PRECISION) AS NUMERIC_PRECISION
		            , c.NUMERIC_SCALE
		            , COALESCE((
			            SELECT 'YES'
			            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
			            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
				            ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
				            AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
			            WHERE c.TABLE_CATALOG = ku.TABLE_CATALOG
				            AND c.TABLE_SCHEMA = ku.TABLE_SCHEMA
				            AND c.TABLE_NAME = ku.TABLE_NAME
				            AND c.COLUMN_NAME = ku.COLUMN_NAME
		            ), 'NO') AS IS_PRIMARY_KEY
            FROM INFORMATION_SCHEMA.TABLES t
            INNER JOIN INFORMATION_SCHEMA.COLUMNS c
	            ON t.TABLE_CATALOG = c.TABLE_CATALOG
	            AND t.TABLE_SCHEMA = c.TABLE_SCHEMA
	            AND t.TABLE_NAME = c.TABLE_NAME
            WHERE t.TABLE_CATALOG = @targetDatabase
	            AND t.TABLE_SCHEMA = @targetSchema
            ORDER BY t.TABLE_NAME, c.ORDINAL_POSITION";

            var sqlParameters = new[]
            {
                new SqlParameter("targetDatabase", targetDatabase),
                new SqlParameter("targetSchema", targetSchema)
            };

            var tableDefinitions = new HashSet<TableMetadata>();
            var dataSet = _dataService.QueryForDataSet(connectionKey, sql, sqlParameters, logResults: true);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                var tableDatabase = row["TABLE_CATALOG"].ToString();
                var tableName = row["TABLE_NAME"].ToString();
                var tableSchema = row["TABLE_SCHEMA"].ToString();
                var columnName = row["COLUMN_NAME"].ToString();
                var ordinalPosition = (int)row["ORDINAL_POSITION"];
                var columnDefault = row["COLUMN_DEFAULT"] == DBNull.Value ? null : row["COLUMN_DEFAULT"].ToString();
                var isNullable = row["IS_NULLABLE"].ToString().Equals("YES");
                var dataType = row["DATA_TYPE"].ToString();
                var maxLength = (int)(row["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value ? -1 : row["CHARACTER_MAXIMUM_LENGTH"]);
                var numericPrecision = (int)(row["NUMERIC_PRECISION"] == DBNull.Value ? -1 : row["NUMERIC_PRECISION"]);
                var numericScale = (int)(row["NUMERIC_SCALE"] == DBNull.Value ? -1 : row["NUMERIC_SCALE"]);
                var isPrimaryKey = row["IS_PRIMARY_KEY"].ToString().Equals("YES");

                var tableDefinition = tableDefinitions.FirstOrDefault(td =>
                    td.Database.Equals(tableDatabase)
                    && td.Schema.Equals(tableSchema)
                    && td.TableName.Equals(tableName));

                var alreadyInSet = tableDefinition != null;
                if (!alreadyInSet) tableDefinition = new TableMetadata(tableDatabase, tableSchema, tableName);

                var columnDefinition = tableDefinition.Columns.FirstOrDefault(tc => tc.ColumnName.Equals(columnName));
                if (columnDefinition != null) continue;

                columnDefinition = new ColumnMetadata
                {
                    Database = tableDatabase,
                    Schema = tableSchema,
                    TableName = tableName,
                    ColumnName = columnName,
                    OrdinalPosition = ordinalPosition,
                    ColumnDefault = columnDefault,
                    IsNullable = isNullable,
                    DataType = dataType,
                    MaxLength = maxLength,
                    NumericPrecision = numericPrecision,
                    NumericScale = numericScale,
                    IsPrimaryKey = isPrimaryKey
                };

                tableDefinition.Columns.Add(columnDefinition);

                if (!alreadyInSet) tableDefinitions.Add(tableDefinition);
            }

            return tableDefinitions;
        }

        private ISet<TableMetadata> UpdateTableRelationships(string connectionKey, string targetDatabase, string targetSchema, ISet<TableMetadata> tableDefinitions)
        {
            Logger.Info($"Retrieving relationship definitions from {targetDatabase} for schema '{targetSchema}'.");

            const string sql = @"
            SELECT  ccu.TABLE_CATALOG
		            , ccu.TABLE_SCHEMA
		            , ccu.constraint_name AS CONSTRAINT_NAME
		            , ccu.table_name AS SOURCE_TABLE_NAME
		            , ccu.column_name AS SOURCE_COLUMN_NAME
		            , kcu.table_name AS TARGET_TABLE_NAME
		            , kcu.column_name AS TARGET_COLUMN_NAME
            FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu
            INNER JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc
                ON ccu.CONSTRAINT_NAME = rc.CONSTRAINT_NAME 
            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu 
                ON kcu.CONSTRAINT_NAME = rc.UNIQUE_CONSTRAINT_NAME
            WHERE ccu.TABLE_CATALOG = @targetDatabase AND ccu.TABLE_SCHEMA = @targetSchema
            ORDER BY ccu.table_name";

            var sqlParameters = new[]
            {
                new SqlParameter("targetDatabase", targetDatabase),
                new SqlParameter("targetSchema", targetSchema)
            };

            var dataSet = _dataService.QueryForDataSet(connectionKey, sql, sqlParameters, logResults: true);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                var tableDatabase = row["TABLE_CATALOG"].ToString();
                var tableSchema = row["TABLE_SCHEMA"].ToString();
                var constraintName = row["CONSTRAINT_NAME"].ToString();
                var sourceTableName = row["SOURCE_TABLE_NAME"].ToString();
                var sourceColumnName = row["SOURCE_COLUMN_NAME"].ToString();
                var targetTableName = row["TARGET_TABLE_NAME"].ToString();
                var targetColumnName = row["TARGET_COLUMN_NAME"].ToString();

                var sourceTableDefinition = tableDefinitions.FirstOrDefault(td =>
                    td.Database.Equals(tableDatabase)
                    && td.Schema.Equals(tableSchema)
                    && td.TableName.Equals(sourceTableName));

                var targetTableDefinition = tableDefinitions.FirstOrDefault(td =>
                    td.Database.Equals(tableDatabase)
                    && td.Schema.Equals(tableSchema)
                    && td.TableName.Equals(targetTableName));

                if (sourceTableDefinition is null || targetTableDefinition is null) continue;

                var sourceColumnDefinition = sourceTableDefinition.Columns.FirstOrDefault(tc => tc.ColumnName.Equals(sourceColumnName));
                var targetColumnDefinition = targetTableDefinition.Columns.FirstOrDefault(tc => tc.ColumnName.Equals(targetColumnName));

                var relationship = new RelationshipMetadata
                {
                    ConstraintName = constraintName,
                    SourceTableName = sourceTableName,
                    SourceColumnName = sourceColumnName,
                    TargetTableName = targetTableName,
                    TargetColumnName = targetColumnName,
                    SourceTable = sourceTableDefinition,
                    SourceColumn = sourceColumnDefinition,
                    TargetTable = targetTableDefinition,
                    TargetColumn = targetColumnDefinition
                };

                var sourceRelationship = sourceTableDefinition.Relationships.FirstOrDefault(r => r.ConstraintName.Equals(constraintName));
                var targetRelationship = targetTableDefinition.Relationships.FirstOrDefault(r => r.ConstraintName.Equals(constraintName));

                if (sourceRelationship is null) sourceTableDefinition.Relationships.Add(relationship);
                if (targetRelationship is null) targetTableDefinition.Relationships.Add(relationship);
            }

            return tableDefinitions;
        }

        private static string BuildDictionaryKey(string targetDatabase, string targetSchema)
        {
            return $"[{targetDatabase}].[{targetSchema}]";
        }
    }
}