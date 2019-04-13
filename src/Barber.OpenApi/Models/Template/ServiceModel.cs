namespace Barber.OpenApi.Models.Template
{
    using Barber.OpenApi.Models;

    public class ServiceModel : IRenderModel
    {
        public ServiceModel()
        {
        }

        public FileModel File { get; set; } = null;

        public string Name { get; set; }

        public PathModel[] Paths { get; set; }

        public ReferenceModel[] References { get; set; }
    }
}
