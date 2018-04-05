namespace CodeGeneration.Models.Configuration
{
    public class ApplicationOptions
    {
        public bool GenerateModels { get; set; }
        public bool GenerateControllers { get; set; }
        public bool GenerateRepositories { get; set; }
        public bool GenerateViews { get; set; }
        public bool GenerateSql { get; set; }
        public string SourceConnectionKey { get; set; }
        public string SourceDatabase { get; set; }
        public string SourceSchema { get; set; }
        public string TargetConnectionKey { get; set; }
        public string TargetDatabase { get; set; }
        public string TargetSchema { get; set; }
        public string[] ReadOnlyProperties { get; set; }
        public string[] RootTemplateDirectories { get; set; }
        public ModelGenerationOptions ModelGeneration { get; set; }
        public ViewGenerationOptions ViewGeneration { get; set; }
        public SqlGenerationOptions SqlGeneration { get; set; }
        public ControllerGenerationOptions ControllerGeneration { get; set; }
        public RepositoryGenerationOptions RepositoryGeneration { get; set; }

        public ApplicationOptions()
        {
            ModelGeneration = new ModelGenerationOptions();
            ViewGeneration = new ViewGenerationOptions();
            SqlGeneration = new SqlGenerationOptions();
            ControllerGeneration = new ControllerGenerationOptions();
            RepositoryGeneration = new RepositoryGenerationOptions();
        }
    }
}
