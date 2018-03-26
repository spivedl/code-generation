using System;

namespace CodeGeneration.Extensions
{
    public static class SqlDataTypeConverter
    {
        public static Type GetType(string sqlType)
        {
            switch (sqlType.ToLower())
            {
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "varchar":
                case "text":
                    return typeof(string);
                case "binary":
                case "timestamp":
                case "varbinary":
                    return typeof(byte[]);
                case "bit":
                    return typeof(bool);
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "time":
                    return typeof(DateTime);
                case "int":
                    return typeof(int);
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney":
                    return typeof(decimal);
                case "bigint":
                    return typeof(long);
                case "tinyint":
                    return typeof(byte);
                case "float":
                    return typeof(double);
                case "smallint":
                    return typeof(short);
                case "real":
                    return typeof(float);
                case "uniqueidentifier":
                    return typeof(Guid);
                default:
                    return typeof(string);
            }
        }

        public static string GetTypeAsString(string sqlType)
        {
            switch (sqlType.ToLower())
            {
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "varchar":
                case "text":
                    return "string";
                case "binary":
                case "timestamp":
                case "varbinary":
                    return "byte[]";
                case "bit":
                    return "bool";
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "time":
                    return "DateTime";
                case "int":
                    return "int";
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney":
                    return "decimal";
                case "bigint":
                    return "long";
                case "tinyint":
                    return "byte";
                case "float":
                    return "double";
                case "smallint":
                    return "short";
                case "real":
                    return "float";
                case "uniqueidentifier":
                    return "Guid";
                default:
                    return "string";
            }
        }
    }
}
