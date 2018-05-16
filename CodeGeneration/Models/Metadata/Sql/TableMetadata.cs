using System.Collections.Generic;

namespace CodeGeneration.Models.Metadata.Sql
{
    public class TableMetadata
    {
        public string SourceDatabase { get; set; }
        public string SourceSchema { get; set; }
        public string TargetDatabase { get; set; }
        public string TargetSchema { get; set; }
        public string TableName { get; set; }
        public List<ColumnMetadata> Columns { get; set; }
        public List<RelationshipMetadata> Relationships { get; set; }
        public Dictionary<string, string> TableAbbreviations { get; set; }

        public TableMetadata() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        public TableMetadata(string sourceDatabase, string sourceSchema, string tableName) : this(sourceDatabase, sourceSchema, sourceDatabase, sourceSchema, tableName)
        {
        }

        public TableMetadata(string sourceDatabase, string sourceSchema, string targetDatabase, string targetSchema, string tableName)
        {
            SourceDatabase = sourceDatabase;
            SourceSchema = sourceSchema;
            TargetDatabase = targetDatabase;
            TargetSchema = targetSchema;
            TableName = tableName;

            Columns = new List<ColumnMetadata>();
            Relationships = new List<RelationshipMetadata>();
            TableAbbreviations = new Dictionary<string, string>();
        }
    }
}
