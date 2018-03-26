using System.Collections.Generic;

namespace CodeGeneration.Models.Metadata.Sql
{
    public class TableMetadata
    {
        public string Database { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public List<ColumnMetadata> Columns { get; set; }
        public List<RelationshipMetadata> Relationships { get; set; }
        public Dictionary<string, string> TableAbbreviations { get; set; }

        public TableMetadata()
        {
            Columns = new List<ColumnMetadata>();
            Relationships = new List<RelationshipMetadata>();
            TableAbbreviations = new Dictionary<string, string>();
        }

        public TableMetadata(string database, string schema, string tableName)
        {
            Database = database;
            Schema = schema;
            TableName = tableName;

            Columns = new List<ColumnMetadata>();
            Relationships = new List<RelationshipMetadata>();
            TableAbbreviations = new Dictionary<string, string>();
        }
    }
}
