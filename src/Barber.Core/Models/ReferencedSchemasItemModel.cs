namespace Barber.Core.Models
{
    public class ReferencedSchemasItemModel
    {
        public ReferencedSchemasItemModel()
        {
        }

        public ReferencedSchemasItemModel(string key, string name)
        {
            this.Key = key;
            this.Name = name;
        }

        public string Key { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string File { get; set; } = string.Empty;
    }
}
