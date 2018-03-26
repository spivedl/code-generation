namespace CodeGeneration.Services.Generation
{
    public interface IGeneratorService<in T>
    {
        void Generate(T context);
        string Get(string modelName, string templateName);
    }
}