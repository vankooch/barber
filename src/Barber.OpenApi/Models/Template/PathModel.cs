namespace Barber.OpenApi.Models.Template
{
    using System.Collections.Generic;
    using Barber.OpenApi.Models;

    public class PathModel : BaseModel
    {
        public PathModel()
        {
        }

        public string Body { get; set; } = null;

        public IReadOnlyList<PropertyModel> Parameters { get; set; }

        public string ParametersFunction
        {
            get
            {
                if (this.Parameters == null || this.Parameters.Count == 0)
                {
                    if (!string.IsNullOrEmpty(this.Body))
                    {
                        return "model: " + this.Body;
                    }

                    return null;
                }

                var text = string.Empty;
                foreach (var item in this.Parameters)
                {
                    text += $", {item.Name}: {item.Type}";
                }

                if (!string.IsNullOrEmpty(this.Body))
                {
                    text += ", model: " + this.Body;
                }

                return text.Trim().Trim(',').Trim();
            }
        }

        public string Path { get; set; }

        public string ReturnType { get; set; }

        public IReadOnlyList<string> Tags { get; set; }

        public string Type { get; set; }

        public string Url
        {
            get
            {
                var text = this.Path;
                text = text.Replace("{version}", "${this.version}");

                if (this.Parameters == null || this.Parameters.Count == 0)
                {
                    return text;
                }

                foreach (var item in this.Parameters)
                {
                    text = text.Replace($"{{{item.Name}}}", $"${{{item.Name}}}");
                }

                return text.Trim();
            }
        }
    }
}
