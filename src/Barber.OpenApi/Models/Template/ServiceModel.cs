namespace Barber.OpenApi.Models.Template
{
    using System.Collections.Generic;
    using Barber.OpenApi.Models;

    public class ServiceModel : IRenderModel
    {
        public ServiceModel()
        {
        }

        public FileModel File { get; set; } = null;

        public string Name { get; set; }

        public IReadOnlyList<PathModel> Paths { get; set; }

        public IReadOnlyList<ReferenceModel> References { get; set; }
    }
}
