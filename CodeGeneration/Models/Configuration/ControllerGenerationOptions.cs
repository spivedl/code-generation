namespace CodeGeneration.Models.Configuration
{
    public class ControllerGenerationOptions
    {
        public string[] TemplateDirectories { get; set; }
        public string[] TemplateNames { get; set; }
        public OutputOptions Output { get; set; }

        public ControllerGenerationOptions()
        {
            TemplateDirectories = new string[] { };
            TemplateNames = new string[] { };
            Output = new OutputOptions();
        }
    }
}
