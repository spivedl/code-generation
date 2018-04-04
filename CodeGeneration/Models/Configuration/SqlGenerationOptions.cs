namespace CodeGeneration.Models.Configuration
{
    public class SqlGenerationOptions
    {
        public string[] TemplateDirectories { get; set; }
        public string[] TemplateNames { get; set; }
        public OutputOptions Output { get; set; }
        public SqlCommandOptions SqlCommand { get; set; }

        public SqlGenerationOptions()
        {
            TemplateDirectories = new string[]{};
            TemplateNames = new string[]{};
            Output = new OutputOptions();
            SqlCommand = new SqlCommandOptions();
        }
    }
}
