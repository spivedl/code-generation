using CodeGeneration.Extensions;

namespace CodeGeneration.Models.Metadata.Sql
{
    public class ColumnMetadata
    {
        public string Database { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsReadOnly { get; set; }
        public int OrdinalPosition { get; set; }
        public string ColumnDefault { get; set; }
        public bool IsNullable { get; set; }
        public string DataType { get; set; }
        public int MaxLength { get; set; }
        public int NumericPrecision { get; set; }
        public int NumericScale { get; set; }

        public ColumnMetadata()
        {
            OrdinalPosition = -1;
            MaxLength = -1;
            NumericPrecision = -1;
            NumericScale = -1;
        }

        public string GetParameterString(bool isForSearch = false)
        {
            var dataType = GetParameterDataType();
            var defaultValue = GetParameterDefaultValue(isForSearch);

            return $"@{ColumnName} {dataType}{defaultValue}";
        }

        private string GetParameterDataType()
        {
            var dataTypeUpper = DataType.ToUpperInvariant();

            // handle length for nvarchar, varchar, nchar, char columns
            if (MaxLength >= 0)
            {
                return $"{dataTypeUpper}({MaxLength})";
            }

            // handle precision and scale for non-integer/non-money numeric columns
            if (NumericScale >= 0
                && !DataType.Contains("int")
                && !DataType.Equals("money"))
            {

                return $"{dataTypeUpper} ({NumericPrecision},{NumericScale})";
            }

            // default case is just append the data type
            return $"{dataTypeUpper}";
        }

        private string GetParameterDefaultValue(bool isForSearch = false)
        {
            if (string.IsNullOrWhiteSpace(ColumnDefault))
            {
                if (IsNullable || isForSearch) return " = NULL";
                return "";
            }

            if (ColumnDefault.Contains("getdate") || ColumnDefault.Contains("getutcdate"))
            {
                return isForSearch ? " = NULL" : "";
            }

            return $" = {ColumnDefault.Replace("(","").Replace(")","")}";
        }

        public bool IsBooleanType()
        {
            return SqlDataTypeConverter.GetType(DataType) == typeof(bool);
        }
    }
}
