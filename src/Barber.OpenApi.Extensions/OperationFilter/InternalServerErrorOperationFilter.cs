namespace Barber.OpenApi.Extensions.OperationFilter
{
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Add 500 - Internal Server Error to every operation response
    /// </summary>
    public class InternalServerErrorOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InternalServerErrorOperationFilter()
        {
        }

        /// <summary>
        /// Apply filter
        /// </summary>
        /// <param name="operation">OpenApi Operation</param>
        /// <param name="context">Context</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses.Add("500", new OpenApiResponse { Description = "Internal Server Error" });
        }
    }
}
