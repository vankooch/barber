namespace Barber.OpenApi.Extensions.OperationFilter
{
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Set OpenApi Operation Summary from Method Name
    /// </summary>
    public class DisplayNameOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DisplayNameOperationFilter()
        {
        }

        /// <summary>
        /// Apply filter
        /// </summary>
        /// <param name="operation">OpenApi Operation</param>
        /// <param name="context">Context</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation != null && context != null)
            {
                operation.Summary = context.MethodInfo.Name;
            }
        }
    }
}
