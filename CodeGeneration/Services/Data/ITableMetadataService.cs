using System.Collections.Generic;
using CodeGeneration.Models.Metadata.Sql;

namespace CodeGeneration.Services.Data
{
    public interface ITableMetadataService
    {
        ISet<TableMetadata> GetTableMetadata(string connectionKey, string targetDatabase, string targetSchema);
    }
}