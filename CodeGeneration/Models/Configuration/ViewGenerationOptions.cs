namespace CodeGeneration.Models.Configuration
{
    public class ViewGenerationOptions
    {
        public string[] TemplateDirectories { get; set; }
        public string[] TemplateNames { get; set; }
        public string[] ReadOnlyProperties { get; set; }
        public OutputOptions Output { get; set; }

        public ViewGenerationOptions()
        {
            Output = new OutputOptions();
        }
    }
}
