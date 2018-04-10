# CodeGeneration TODO

## TODO

+ Add an option for static templates (AKA ICrudRepository)
+ Create `MetadataModel` classes for each generator type to make templates for controllers, repositories, etc. easier to write.
+ Add namespace and using as configuration options for all generators.
+ I might be able to create some type of GenericGeneratorService for everything other than the ModelGeneratorService because as of 4/4/2018, all other generators are very, very similar.
+ Combine all *GenerationContext and *GenerationOptions classes into a single class. As of right now, every one of those classes has the same properties.
+ Rename the IBootService and BootService to something more appropriate.
+ Right now, code generation (sql, views, etc.) is dependent upon the models being generated first. I need to update the model services 
so that it will lazy generate models if necessary, this will allow for each type of code generation to be run independently.
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
+ Add `FileWriter` usage and settings for ModelGeneration and ViewGeneration
+ Fix RazorTemplateService GetEmbeddedTemplateNames* methods so that they filter by template name correctly. I'm using contains right now, so it behaves a little unexpected.

## CANCELLED

+ Create a generic `GeneratorService` that uses table definitions - instead of models - along with templates to create output.
  + This should remove the need for a specific 'generator' for each type since the C# code for each generator is very similar.
  + This could also remove the need for compiling things in memory, since we should be able to use `TableMetadata` and `ColumnMetadata` to build template models.
  + Main difference between generator classes right now is the WriteToFile method, so probably need to create a `context` object to determine how a file is named, etc. Also, add to the interface.