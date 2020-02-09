namespace Barber.OpenApi.Generator
{
    using System.Collections.Generic;
    using Barber.OpenApi.Models.Template;
    using Microsoft.OpenApi.Models;

    /// <summary>
    /// Generator for handling OpenApi Paths section
    /// </summary>
    public interface IGeneratorPaths
    {
        /// <summary>
        /// Convert a Media Type to string value
        /// </summary>
        /// <param name="response">OpenApi Media Type</param>
        /// <returns></returns>
        string? GetMediaType(KeyValuePair<string, OpenApiMediaType>? response);

        /// <summary>
        /// Convert a Parameter to into intermediate Property Model
        /// </summary>
        /// <param name="parameter">OpenApi parameter</param>
        /// <returns></returns>
        PropertyModel? GetParameter(OpenApiParameter? parameter);

        /// <summary>
        /// Convert a Path to into intermediate Path Model
        /// </summary>
        /// <param name="path">OpenApi path</param>
        /// <param name="operation">OpenApi operation</param>
        /// <returns></returns>
        PathModel? GetPath(KeyValuePair<string, OpenApiPathItem>? path, KeyValuePair<OperationType, OpenApiOperation>? operation);

        /// <summary>
        /// Convert a Service to into intermediate Path Service
        /// </summary>
        /// <param name="tag">Tag Name</param>
        /// <param name="paths">OpenApi path list</param>
        /// <returns></returns>
        ServiceModel? GetService(string? tag, IEnumerable<PathModel>? paths);
    }
}
