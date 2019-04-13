namespace Barber.OpenApi.Models
{
    using Newtonsoft.Json;

    public class FileModel
    {
        public FileModel()
        {
        }

        [JsonIgnore]
        public string Content { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string Template { get; set; } = string.Empty;
    }
}
