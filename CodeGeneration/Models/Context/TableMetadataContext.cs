using CodeGeneration.Models.Configuration;

namespace CodeGeneration.Models.Context
{
    public class TableMetadataContext
    {
        public string ConnectionKey { get; set; }
        public string TargetDatabase { get; set; }
        public string TargetSchema { get; set; }
        public string[] ReadOnlyColumns { get; set; }

        public TableMetadataContext()
        {
            
        }

        public TableMetadataContext(string connectionKey, string targetDatabase, string targetSchema, string[] readOnlyColumns)
        {
            ConnectionKey = connectionKey;
            TargetDatabase = targetDatabase;
            TargetSchema = targetSchema;
            ReadOnlyColumns = readOnlyColumns;
        }
    }
}
