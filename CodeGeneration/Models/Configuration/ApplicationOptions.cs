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
        public GenerationOptions ModelGeneration { get; set; }
        public GenerationOptions ViewGeneration { get; set; }
        public GenerationOptions SqlGeneration { get; set; }
        public GenerationOptions ControllerGeneration { get; set; }
        public GenerationOptions RepositoryGeneration { get; set; }

        public ApplicationOptions()
        {
            ControllerGeneration = new GenerationOptions();
            ModelGeneration = new GenerationOptions();
            SqlGeneration = new GenerationOptions();
            RepositoryGeneration = new GenerationOptions();
            ViewGeneration = new GenerationOptions();
        }
    }
}
