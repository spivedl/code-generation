using System;
using System.Collections.Generic;
using System.Linq;
using CodeGeneration.Extensions;
using CodeGeneration.Models.Metadata.Sql;

namespace CodeGeneration.Models.Template
{
    public class ControllerTemplateModel
    {
        public string TargetNamespace { get; set; }
        public string RootNamespace { get; set; }
        public string ModelNamespace { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string CreateDate { get; set; }
        public string ModelName { get; set; }
        public string ClassName { get; set; }
        public string ConstructorParameters { get; set; }
        public ColumnMetadata PrimaryKeyColumn { get; set; }
        public IEnumerable<ColumnMetadata> ForeignKeyColumns { get; set; }

        public ControllerTemplateModel(string targetNamespace, TableMetadata tableMetadata)
        {
            TargetNamespace = targetNamespace;
            RootNamespace = targetNamespace.Substring(0, targetNamespace.IndexOf('.'));
            ModelNamespace = $"{RootNamespace}.Models.Domain";
            Author = RazorEngineExtensions.Author();
            Version = RazorEngineExtensions.Version();
            CreateDate = DateTime.Now.ToString("f");
            ModelName = tableMetadata.TableName.ToCamelCase();
            ClassName = $"{ModelName}Controller";
            PrimaryKeyColumn = tableMetadata.Columns.FirstOrDefault(c => c.IsPrimaryKey);
            ForeignKeyColumns = tableMetadata.Columns.Where(c => c.IsForeignKey);

            var primaryRepositoryCtor = $"{ModelName.ToInterfaceName("Repository")} {ModelName.ToVariableName("Repository")}";
            var foreignRepositoryCtor = string.Join(", ",
                ForeignKeyColumns
                    .Select(c => c.ColumnName.Replace("Id", ""))
                    .Select(columnName => $"{columnName.ToInterfaceName("Repository")} {columnName.ToVariableName("Repository")}"));

            ConstructorParameters = primaryRepositoryCtor;
            if (!string.IsNullOrWhiteSpace(foreignRepositoryCtor)) ConstructorParameters += $", {foreignRepositoryCtor}";
        }
    }
}
