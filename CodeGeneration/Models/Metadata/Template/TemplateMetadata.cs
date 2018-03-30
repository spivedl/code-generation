namespace CodeGeneration.Models.Metadata.Template
{
    public class TemplateMetadata
    {
        public string ModelName { get; set; }
        public string TemplateName { get; set; }
        public string CacheKey => $"{ModelName}:{TemplateName}";
        public string TemplateText { get; set; }
        public string ProcessedTemplateText { get; set; }
    }
}
