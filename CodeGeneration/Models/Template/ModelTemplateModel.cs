using System.Collections.Generic;
using CodeGeneration.Extensions;
using CodeGeneration.Models.Context;
using CodeGeneration.Models.Metadata.Model;
using CodeGeneration.Models.Metadata.Sql;

namespace CodeGeneration.Models.Template
{
    public class ModelTemplateModel : TemplateModel
    {
        public string ModelNamespace { get; set; }
        public ISet<PropertyMetadata> PropertyMetadataSet { get; set; }

        public ModelTemplateModel()
        {
            PropertyMetadataSet = new HashSet<PropertyMetadata>();
        }

        public ModelTemplateModel(TemplateModelContext context) : base(context.TableMetadata)
        {
            ModelNamespace = context.ApplicationOptions.ModelGeneration.Namespace;
            CreatePropertyMetadataSet(context.TableMetadata);
        }

        public void CreatePropertyMetadataSet(TableMetadata tableMetadata)
        {
            PropertyMetadataSet = new HashSet<PropertyMetadata>();

            foreach (var column in tableMetadata.Columns)
            {
                var columnType = SqlDataTypeConverter.GetType(column.DataType);

                PropertyMetadataSet.Add(new PropertyMetadata
                {
                    PropertyName = column.ColumnName,
                    DisplayName = column.ColumnName.ToCamelCase(" "),
                    TypeName = columnType.FullName,
                    ShortTypeName = SqlDataTypeConverter.GetTypeAsString(column.DataType),
                    IsReadOnly = column.IsReadOnly,
                    IsNullableType = SqlDataTypeConverter.IsNullableType(column),
                    IsPrimaryKey = column.IsPrimaryKey,
                    IsForeignKey = column.IsForeignKey,
                    ItemSourceName = column.ColumnName.Replace("Id", "List")
                });
            }
        }
    }
}
