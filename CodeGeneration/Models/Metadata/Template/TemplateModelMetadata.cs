using CodeGeneration.Extensions;
using CodeGeneration.Models.Metadata.Sql;

namespace CodeGeneration.Models.Metadata.Template
{
    public class TemplateModelMetadata
    {
        public string SourceConnectionKey { get; set; }
        public string Namespace { get; set; }
        public string ModelName => TableMetadata?.TableName.ToCamelCase();
        public TableMetadata TableMetadata { get; set; }

        public TemplateModelMetadata()
        {
            
        }

        public TemplateModelMetadata(string connectionKey, string ns, TableMetadata tableMetadata)
        {
            SourceConnectionKey = connectionKey;
            Namespace = ns;
            TableMetadata = tableMetadata;
        }
    }
}
