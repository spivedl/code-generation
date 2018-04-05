namespace CodeGeneration.Models.Configuration
{
    public class ModelGenerationOptions
    {
        public string[] TemplateDirectories { get; set; }
        public string[] TemplateNames { get; set; }
        public OutputOptions Output { get; set; }

        public ModelGenerationOptions()
        {
            TemplateDirectories = new string[] { };
            TemplateNames = new string[] { };
            Output = new OutputOptions();
        }
    }
}
