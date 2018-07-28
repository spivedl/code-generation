using CodeGeneration.Models.Configuration;

namespace CodeGeneration.Models.Context
{
    public class OutputContext
    {
        public OutputOptions OutputOptions { get; set; }
        public string EntityName { get; set; }
        public string ResourceName { get; set; }
        public string TemplateName { get; set; }
        public string TemplateContents { get; set; }
    }
}
