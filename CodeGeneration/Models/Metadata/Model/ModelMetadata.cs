using System;
using System.Collections.Generic;
using System.Linq;
using CodeGeneration.Models.Domain.Attributes;

namespace CodeGeneration.Models.Metadata.Model
{
    public class ModelMetadata
    {
        public bool IsPartialView { get; set; }
        public bool IsLayoutPageSelected { get; set; }
        public string LayoutPageFile { get; set; }
        public string ViewName { get; set; }
        public string ViewDataTypeName { get; set; }
        public string ViewDataTypeShortName { get; set; }
        public string Header { get; set; }
        public string Title { get; set; }

        public ISet<PropertyMetadata> Properties { get; set; }

        public ModelMetadata(Type model) : this(false, true, null, "", model)
        {
        }

        public ModelMetadata(string viewName, Type model) : this(false, true, null, viewName, model)
        {
        }

        public ModelMetadata(bool isPartialView, bool isLayoutPageSelected, string layoutPageFile, string viewName, Type model)
        {
            IsPartialView = isPartialView;
            IsLayoutPageSelected = isLayoutPageSelected;
            LayoutPageFile = layoutPageFile;
            ViewName = viewName;
            ViewDataTypeName = model.FullName;
            ViewDataTypeShortName = model.Name;
            Title = $"{viewName} {model.Name}";
            Properties = GetProperties(model);

            //Header = DetermineHeader(IsPartialView, IsLayoutPageSelected, LayoutPageFile, Title);
        }

        private static ISet<PropertyMetadata> GetProperties(Type model)
        {
            var set = new HashSet<PropertyMetadata>();

            foreach (var p in model.GetProperties())
            {
                var attributes = p.GetCustomAttributes(true);
                var isPrimaryKey = attributes.FirstOrDefault(a => a.GetType() == typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;
                var isForeignKey = attributes.FirstOrDefault(a => a.GetType() == typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;
                var itemSourceName = isForeignKey?.ItemSourceName;
                var isReadonly = attributes.FirstOrDefault(a => a.GetType() == typeof(ReadOnlyAttribute)) as ReadOnlyAttribute;
                var scaffold = attributes.FirstOrDefault(a => a.GetType() == typeof(ScaffoldAttribute)) as ScaffoldAttribute;
                var displayName = attributes.FirstOrDefault(a => a.GetType() == typeof(DisplayNameAttribute)) as DisplayNameAttribute;

                set.Add(new PropertyMetadata
                {
                    PropertyName = p.Name,
                    TypeName = p.PropertyType.FullName,
                    ShortTypeName = p.PropertyType.Name,
                    IsPrimaryKey = isPrimaryKey?.IsPrimaryKey ?? false,
                    IsForeignKey = isForeignKey?.IsForeignKey ?? false,
                    ItemSourceName = itemSourceName,
                    IsReadOnly = isReadonly?.IsReadOnly ?? false,
                    Scaffold = scaffold?.ScaffoldProperty ?? true,
                    DisplayName = displayName?.DisplayName ?? p.Name,
                });
            }

            return set;
        }

        /*private static string DetermineHeader(bool isPartialView, bool isLayoutPageSelected, string layoutPageFile, string title)
        {
            if (isPartialView) return "";

            var sb = new StringBuilder();
            if (isLayoutPageSelected)
            {
                sb
                    .AppendLine("@{")
                    .AppendLine($"\tViewData[\"Title\"] = \"{title}\";");

                if (!string.IsNullOrWhiteSpace(layoutPageFile))
                {
                    sb.AppendLine($"\tLayout = \"{layoutPageFile}\";");
                }

                sb.AppendLine("}");
            }
            else
            {
                sb
                    .AppendLine("@{")
                    .AppendLine("\tLayout = null;")
                    .AppendLine("}")
                    .AppendLine()
                    .AppendLine("<!DOCTYPE html>")
                    .AppendLine()
                    .AppendLine("<html>")
                    .AppendLine("<head>")
                    .AppendLine("\t<meta name=\"viewport\" content=\"width=device-width\"/>")
                    .AppendLine($"\t<title>{title}</title>")
                    .AppendLine("</head>")
                    .AppendLine("<body>");
            }

            return sb.ToString();
        }*/
    }
}
