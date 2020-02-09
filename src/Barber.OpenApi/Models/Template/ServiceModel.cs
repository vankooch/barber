namespace Barber.OpenApi.Models.Template
{
    using System.Collections.Generic;
    using Barber.OpenApi.Models;

    public class ServiceModel : BaseModel, IRenderModel
    {
        public ServiceModel()
        {
        }

        public FileModel? File { get; set; } = null;

        public IReadOnlyList<PathModel>? Paths { get; set; }
    }
}
