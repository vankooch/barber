namespace Barber.OpenApi.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Barber.OpenApi.Models.Template;

    public abstract class BaseModel
    {
        public BaseModel()
        {
        }

        public string Name { get; set; }

        public IReadOnlyList<ReferenceModel> References { get; set; }

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
