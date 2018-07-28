using CodeGeneration.Models.Metadata.Sql;

namespace CodeGeneration.Models.Context
{
    public class TemplateModelContext
    {
        public string ConnectionKey { get; set; }
        public string TargetNamespace { get; set; }
        public TableMetadata TableMetadata { get; set; }
    }
}
