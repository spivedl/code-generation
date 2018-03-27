namespace CodeGeneration.Models.Configuration
{
    public class ModelGenerationOptions
    {
        public string[] TemplateDirectories { get; set; }
        public string[] TemplateNames { get; set; }
        public string[] ReadOnlyProperties { get; set; }
    }
}
