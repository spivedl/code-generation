using System.Collections.Generic;
using CodeGeneration.Models.Context;
using CodeGeneration.Models.Metadata.Sql;

namespace CodeGeneration.Services.Data
{
    public interface ITableMetadataService
    {
        ISet<TableMetadata> GetTableMetadata(TableMetadataContext context);
    }
}