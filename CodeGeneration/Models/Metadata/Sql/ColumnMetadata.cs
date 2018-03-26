namespace CodeGeneration.Models.Metadata.Sql
{
    public class ColumnMetadata
    {
        public string Database { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public bool IsPrimaryKey { get; set; }
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
    }
}
