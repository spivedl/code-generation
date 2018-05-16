using CodeGeneration.Models.Configuration;

namespace CodeGeneration.Models.Context
{
    public class TableMetadataContext
    {
        public ApplicationOptions ApplicationOptions { get; set; }

        public TableMetadataContext() : this(new ApplicationOptions())
        {
        }

        public TableMetadataContext(ApplicationOptions applicationOptions)
        {
            ApplicationOptions = applicationOptions;
        }
    }
}
