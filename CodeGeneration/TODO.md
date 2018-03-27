# CodeGeneration TODO

## TODO

+ Create a service to handle retrieving template files that are compiled as embedded resources.
+ Create extension methods for convenience related to embedded resources.
  + GetTemplateNameFromEmbeddedResource
  +	GetFileNameFromEmbeddedResource
  + GetViewNameFromEmbeddedResource
+ Rename the `AssemblyExtensions` class to something more appropriate for its purpose.
+ Add a `BaseGeneratorService` with an abstract implementation of the `Get` method. 
  + This logic should be the same across all types of generators (model, sql, view, etc.)

## COMPLETED

+ Create a new git repository on Bitbucket for project.