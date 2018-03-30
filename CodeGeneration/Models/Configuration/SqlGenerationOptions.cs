namespace CodeGeneration.Models.Configuration
{
    public class SqlGenerationOptions
    {
        public string[] TemplateDirectories { get; set; }
        public string[] TemplateNames { get; set; }
        public string[] ReadOnlyProperties { get; set; }
        public OutputOptions Output { get; set; }

        public SqlGenerationOptions()
        {
            Output = new OutputOptions();
        }
    }
}
