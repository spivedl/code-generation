using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.Metadata.Sql;

namespace CodeGeneration.Models.Context
{
    public class TemplateModelContext
    {
        public ApplicationOptions ApplicationOptions { get; set; }
        public TableMetadata TableMetadata { get; set; }
    }
}
