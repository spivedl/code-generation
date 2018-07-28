using CodeGeneration.Extensions;
using CodeGeneration.Models.Metadata.Sql;

namespace CodeGeneration.Models.Template
{
    public class TableMetadataTemplateModel : TemplateModel
    {
        public string SourceConnectionKey { get; set; }
        public string TargetNamespace { get; set; }
        public string ModelName => TableMetadata?.TableName.ToCamelCase();
        public string TargetDatabase { get; set; }
        public string TargetSchema { get; set; }
        public TableMetadata TableMetadata { get; set; }

        public TableMetadataTemplateModel()
        {
            
        }

        public TableMetadataTemplateModel(string connectionKey, string ns, TableMetadata tableMetadata)
        {
            SourceConnectionKey = connectionKey;
            TargetNamespace = ns;
            TableMetadata = tableMetadata;
        }
    }
}
