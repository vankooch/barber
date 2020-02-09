namespace Barber.OpenApi.I18next
{
    using System.Collections.Generic;
    using System.Linq;

    public class NamespaceModel
    {
        public NamespaceModel()
        {
        }

        public NamespaceModel(IDictionary<string, string> data) => this.Items = Constants.ConvertToItemGroup(data);

        public IDictionary<string, string> Data
        {
            get
            {
                var merged = new Dictionary<string, string>();
                foreach (var item in this.Items)
                {
                    merged = merged
                        .Concat(item.Data)
                        .ToDictionary(x => x.Key, x => x.Value);
                }

                return merged;
            }
        }

        public List<ItemModel> Items { get; private set; } = new List<ItemModel>();

        public string? Language { get; set; }

        public string? Name { get; set; }

        public void Add(string key, string value)
        {
            if (!this.Items.Any(e => e.Name == key))
            {
                this.Items.Add(new ItemModel(key, value));
            }
        }
    }
}
