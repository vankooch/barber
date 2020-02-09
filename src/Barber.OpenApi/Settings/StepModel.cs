namespace Barber.OpenApi.Settings
{
    using System.Collections.Generic;
    using Barber.OpenApi.Generator;
    using Barber.OpenApi.Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Step definition model
    /// </summary>
    public class StepModel
    {
        public StepModel()
        {
        }

        /// <summary>
        /// Destination path for generated files
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Name of generator to use
        /// </summary>
        public string Generator { get; set; }

        [JsonIgnore]
        public GeneratorType GeneratorType { get; set; } = GeneratorType.Path;

        /// <summary>
        /// Name of paths / schema's to include
        /// </summary>
        public IReadOnlyList<string> ItemsIncludes { get; set; }

        /// <summary>
        /// List of paths / schema's to skip
        /// </summary>
        public IReadOnlyList<string> ItemsSkips { get; set; }

        /// <summary>
        /// Step name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Resolver with help of other steps
        /// </summary>
        public string Resolve { get; set; }

        [JsonIgnore]
        public IDictionary<string, IRenderModel> ResultItems { get; internal set; } = new Dictionary<string, IRenderModel>();

        /// <summary>
        /// Path to template files used for processing
        /// </summary>
        public string Template { get; set; }
    }
}
