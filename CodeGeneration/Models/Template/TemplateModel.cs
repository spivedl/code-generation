using System;
using CodeGeneration.Extensions;
using CodeGeneration.Models.Metadata.Sql;
using DotLiquid;

namespace CodeGeneration.Models.Template
{
    public class TemplateModel : Drop
    {
        public string Author { get; set; }
        public string Version { get; set; }
        public string Now { get; set; }
        public string TableName { get; set; }
        public string EntityName { get; set; }
        public TableMetadata TableMetadata { get; set; }

        public TemplateModel() : this(new TableMetadata())
        {
        }

        public TemplateModel(TableMetadata tableMetadata)
        {
            Author = RazorEngineExtensions.Author();
            Version = RazorEngineExtensions.Version();
            Now = DateTime.Now.ToString("s");

            TableName = tableMetadata.TableName;
            EntityName = TableName.ToCamelCase();
            TableMetadata = tableMetadata;
        }
    }
}
