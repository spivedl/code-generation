namespace CodeGeneration.Models.Metadata.Sql
{
    public class RelationshipMetadata
    {
        public string ConstraintName { get; set; }
        public string SourceTableName { get; set; }
        public string SourceColumnName { get; set; }
        public string TargetTableName { get; set; }
        public string TargetColumnName { get; set; }

        public TableMetadata SourceTable { get; set; }
        public ColumnMetadata SourceColumn { get; set; }
        public TableMetadata TargetTable { get; set; }
        public ColumnMetadata TargetColumn { get; set; }
    }
}
