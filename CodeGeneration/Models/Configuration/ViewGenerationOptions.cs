namespace CodeGeneration.Models.Configuration
{
    public class ViewGenerationOptions
    {
        public string[] TemplateDirectories { get; set; }
        public string[] TemplateNames { get; set; }
        public OutputOptions Output { get; set; }

        public ViewGenerationOptions()
        {
            TemplateDirectories = new string[] { };
            TemplateNames = new string[] { };
            Output = new OutputOptions();
        }
    }
}
