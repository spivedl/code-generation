# CodeGeneration TODO

## TODO

+ Add `FileWriter` usage and settings for ModelGeneration and ViewGeneration
+ Move View Header logic back into the View since it could be View dependent and it belongs there any way.

## COMPLETED

+ I need to revisit caching logic, it's a little confusing to use right now with the `BaseGenerationService` implementation that I have.
+ Create a new git repository on Bitbucket for project.
+ Create extension methods for convenience related to embedded resources.
  + GetTemplateNameFromEmbeddedResource
  +	GetFileNameFromEmbeddedResource
  + GetViewNameFromEmbeddedResource
+ Rename the `AssemblyExtensions` class to something more appropriate for its purpose.
+ Create a service to handle retrieving template files that are compiled as embedded resources.
+ Add a `BaseGeneratorService` with an abstract implementation of the `Get` method. 
  + This logic should be the same across all types of generators (model, sql, view, etc.)
+ Update TableMetadata code to include information about whether a column is ReadOnly, ForeignKey, etc. to make the template simpler.
+ Add caching to the `CSharpInMemoryCompiler` class so that I only have to compile my model classes one time.