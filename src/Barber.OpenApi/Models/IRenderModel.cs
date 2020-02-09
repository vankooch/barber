namespace Barber.OpenApi.Models
{
    using System.Collections.Generic;
    using Barber.OpenApi.Models.Template;

    /// <summary>
    /// Helper interface
    /// </summary>
    public interface IRenderModel
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// File model
        /// </summary>
        FileModel File { get; set; }

        /// <summary>
        /// Used references
        /// </summary>
        IReadOnlyList<ReferenceModel> References { get; set; }
    }
}
