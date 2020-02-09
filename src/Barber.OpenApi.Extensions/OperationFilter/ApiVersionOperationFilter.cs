namespace Barber.OpenApi.Extensions.OperationFilter
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Handle API Versioning you can set default and possible values vie pass trough arguments.
    /// </summary>
    public class ApiVersionOperationFilter : IOperationFilter
    {
        private readonly int _versionDefault;
        private readonly int[] _versions;

        /// <summary>
        /// Constructor
        /// </summary>
        public ApiVersionOperationFilter(int defaultVersion = 1, IEnumerable<int>? versions = null)
        {
            this._versionDefault = defaultVersion;

            if (versions == null)
            {
                this._versions = new int[] { 1 };
            }
            else
            {
                this._versions = versions.ToArray();
            }
        }

        /// <summary>
        /// Apply filter
        /// </summary>
        /// <param name="operation">OpenApi Operation</param>
        /// <param name="context">Context</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null)
            {
                return;
            }

            var apiVersion = operation
                .Parameters
                .FirstOrDefault(e => e.Name == "version" && e.In == ParameterLocation.Path && e.Required);
            if (apiVersion != null)
            {
                for (var i = 0; i < this._versions.Length; i++)
                {
                    apiVersion.Schema.Enum.Add(new OpenApiInteger(this._versions[i]));
                }

                apiVersion.Schema.Default = new OpenApiInteger(this._versionDefault);
            }
        }
    }
}
