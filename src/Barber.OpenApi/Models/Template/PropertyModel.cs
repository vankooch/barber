namespace Barber.OpenApi.Models.Template
{
    using Microsoft.OpenApi.Models;
    using Newtonsoft.Json;

    public class PropertyModel
    {
        public PropertyModel()
        {
        }

        public string DefaultValue { get; set; }

        public string Name { get; set; }

        public bool Nullable { get; set; } = false;

        public string Reference { get; set; } = string.Empty;

        public bool Required { get; set; } = false;

        public string RootSchema { get; set; }

        public OpenApiSchema Schema { get; set; }

        public string SchemaString
        {
            get
            {
                var settings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    ContractResolver = new JsonHelper.IgnoreEmptyEnumerableResolver(),
                };

                return JsonConvert.SerializeObject(this.Schema, Formatting.None, settings);
            }
        }

        public string Type { get; set; }
    }
}
