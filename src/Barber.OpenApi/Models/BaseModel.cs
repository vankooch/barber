namespace Barber.OpenApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Barber.OpenApi.Models.Template;

    public abstract class BaseModel
    {
        public BaseModel()
        {
        }

        public string Name { get; set; } = string.Empty;

        public IReadOnlyList<ReferenceModel> References { get; set; } = Array.Empty<ReferenceModel>();

        public void AddReference(string key, bool checkName = false)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (checkName && key == this.Name)
            {
                return;
            }

            if (this.References == null || this.References.Count == 0)
            {
                this.References = new ReferenceModel[]
                {
                    new ReferenceModel()
                    {
                        Key = key,
                    },
                };

                return;
            }

            if (!this.References.Any(e => e.Key == key))
            {
                var list = this.References.ToList();
                list.Add(new ReferenceModel() { Key = key });

                this.References = list.ToArray();
            }
        }
    }
}
