namespace Barber.Core.Models
{
    using System.Collections.Generic;
    using System.IO;

    public class ReferencedSchemasModel
    {
        public string? FileFull { get; set; }

        public string? File
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.FileFull))
                {
                    return string.Empty;
                }

                return Path.GetFileNameWithoutExtension(this.FileFull);
            }
        }

        public List<ReferencedSchemasItemModel> Imports { get; set; } = new List<ReferencedSchemasItemModel>();
    }
}
