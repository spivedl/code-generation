namespace CodeGeneration.Models.Configuration
{
    public class GenerationOptions
    {
        public string Namespace { get; set; }
        public string[] TemplateDirectories { get; set; }
        public string[] TemplateNames { get; set; }
        public OutputOptions Output { get; set; }
        public SqlCommandOptions SqlCommand { get; set; }

        public GenerationOptions()
        {
            TemplateDirectories = new string[] { };
            TemplateNames = new string[] { };
            Output = new OutputOptions();
            SqlCommand = new SqlCommandOptions();
        }
    }
}
