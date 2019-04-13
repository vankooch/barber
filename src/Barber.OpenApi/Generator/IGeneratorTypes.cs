namespace Barber.OpenApi.Generator
{
    using Microsoft.OpenApi.Models;

    /// <summary>
    /// Common converters
    /// </summary>
    public interface IGeneratorTypes
    {
        /// <summary>
        /// Convert OpenApi type
        /// </summary>
        /// <param name="property">OpenApi property</param>
        /// <param name="setNull">Is null able</param>
        /// <returns></returns>
        string ConvertType(OpenApiSchema property, bool setNull = true);

        /// <summary>
        /// Get default value based on property
        /// </summary>
        /// <param name="property">OpenApi property</param>
        /// <returns></returns>
        string GetDefaultValue(OpenApiSchema property);

        /// <summary>
        /// Get reference string
        /// </summary>
        /// <param name="property">OpenApi property</param>
        /// <returns></returns>
        string GetReference(OpenApiSchema property);

        /// <summary>
        /// Get reference path string
        /// </summary>
        /// <param name="relativePath">OpenApi property</param>
        /// <returns></returns>
        string GetReferencePath(string relativePath);

        /// <summary>
        /// Check if is null able
        /// </summary>
        /// <param name="property">OpenApi property</param>
        /// <returns></returns>
        bool IsNullable(OpenApiSchema property);
    }
}
