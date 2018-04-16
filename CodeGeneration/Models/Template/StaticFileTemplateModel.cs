using System.Collections.Generic;
using CodeGeneration.Models.Metadata.Sql;

namespace CodeGeneration.Models.Template
{
    public class StaticFileTemplateModel
    {
        public string TargetNamespace { get; set; }
        public ISet<TableMetadata> TableMetadataSet { get; set; }
    }
}
