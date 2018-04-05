namespace CodeGeneration.Models.Configuration
{
    public class RepositoryGenerationOptions
    {
        public string[] TemplateDirectories { get; set; }
        public string[] TemplateNames { get; set; }
        public OutputOptions Output { get; set; }

        public RepositoryGenerationOptions()
        {
            TemplateDirectories = new string[] { };
            TemplateNames = new string[] { };
            Output = new OutputOptions();
        }
    }
}
