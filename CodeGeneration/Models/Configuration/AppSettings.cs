using System.IO;

namespace CodeGeneration.Models.Configuration
{
    public class AppSettings
    {
        public bool GenerateSql { get; set; }
        public bool GenerateCSharp { get; set; }
        public SqlGenerationOptions SqlGeneration { get; set; }
        public CSharpGenerationOptions CSharpGeneration { get; set; }

        public AppSettings()
        {
            SqlGeneration = new SqlGenerationOptions();
            CSharpGeneration = new CSharpGenerationOptions();
        }

        public class SqlGenerationOptions
        {
            public string SourceDatabase { get; set; }
            public string SourceSchema { get; set; }
            public string Version { get; set; }
            public string Author { get; set; }
            public string[] ReadOnlyColumns { get; set; }
            public string TemplateDirectory { get; set; }
            public string InsertTemplate { get; set; }
            public string UpdateTemplate { get; set; }
            public string DeleteTemplate { get; set; }
            public string SearchTemplate { get; set; }
            public string OutputDirectory { get; set; }
            public bool OutputOnly { get; set; }
            public SqlCommandOptions SqlCommand { get; set; }

            public SqlGenerationOptions()
            {
                TemplateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "templates", "sql");
                OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output", "sql");
                SqlCommand = new SqlCommandOptions();
            }
        }

        public class SqlCommandOptions
        {
            public string TargetServer { get; set; }
            public string TargetDatabase { get; set; }
            public bool UseTrustedConnection { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
            public bool EchoInput { get; set; }
        }

        public class CSharpGenerationOptions
        {
            public string SourceDatabase { get; set; }
            public string SourceSchema { get; set; }
            public string Version { get; set; }
            public string Author { get; set; }
            public string[] ReadOnlyColumns { get; set; }
            public string ConnectionKey { get; set; }
            public string ModelTemplate { get; set; }
            public string BaseRepositoryTemplate { get; set; }
            public string AbstractRepositoryTemplate { get; set; }
            public string ConcreteRepositoryTemplate { get; set; }
            public string ModelNamespace { get; set; }
            public string RepositoryNamespace { get; set; }
            public string DataNamespace { get; set; }
            public string OutputDirectory { get; set; }
        }
    }
}
