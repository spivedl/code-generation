using System;
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

        public string GetParameterString()
        {
            var dataType = GetParameterDataType();
            var defaultValue = GetParameterDefaultValue();

            return $"@{ColumnName} {dataType}{defaultValue}";
        }

        private string GetParameterDataType()
        {
            var dataTypeUpper = DataType.ToUpperInvariant();

            // handle length for nvarchar, varchar, nchar, char columns
            if (MaxLength >= 0)
            {
                return $"{dataTypeUpper} ({MaxLength})";
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

        private string GetParameterDefaultValue()
        {
            if (IsNullable && string.IsNullOrWhiteSpace(ColumnDefault)) return " = NULL";
            if (!string.IsNullOrWhiteSpace(ColumnDefault) 
                && (ColumnDefault.Contains("getdate") || ColumnDefault.Contains("getutcdate"))) return "";

            return string.IsNullOrWhiteSpace(ColumnDefault) ? "" : $" = {ColumnDefault}";
        }

        public bool IsBooleanType()
        {
            return SqlDataTypeConverter.GetType(DataType) == typeof(bool);
        }
    }
}
