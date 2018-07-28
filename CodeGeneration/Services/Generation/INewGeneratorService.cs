using CodeGeneration.Models.Context;

namespace CodeGeneration.Services.Generation
{
    public interface INewGeneratorService
    {
        GenerationStatistics GenerateOutput(GenerationContext context);
        string GetTemplateContents(string templateName);
    }
}
