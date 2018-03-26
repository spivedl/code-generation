namespace CodeGeneration.Models.Metadata.Model
{
    public class PropertyMetadata
    {
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsReadOnly { get; set; }
        public bool Scaffold { get; set; }
        public string PropertyName { get; set; }
        public string DisplayName { get; set; }
        public string TypeName { get; set; }
        public string ShortTypeName { get; set; }
        public string ItemSourceName { get; set; }
    }
}
