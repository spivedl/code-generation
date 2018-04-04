namespace CodeGeneration.Models.Configuration
{
    public class RepositoryGenerationOptions
    {
        public string[] TemplateDirectories { get; set; }
        public string[] TemplateNames { get; set; }
        public OutputOptions Output { get; set; }

        public RepositoryGenerationOptions()
        {
            Output = new OutputOptions();
        }
    }
}
