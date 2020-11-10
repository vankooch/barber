namespace Barber.IoT.Api.Filters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Net;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// Main Exception Handling on Request
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        public GlobalExceptionFilterAttribute(
            IWebHostEnvironment hostingEnvironment,
            IModelMetadataProvider modelMetadataProvider)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._modelMetadataProvider = modelMetadataProvider;
        }

        /// <summary>
        /// Handle Exception
        /// </summary>
        public override void OnException(ExceptionContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Result = new EmptyResult();

            if (context.Exception is ValidationException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = new JsonResult(
                    ((ValidationException)context.Exception).ValidationResult);

                return;
            }

            if (context.Exception is KeyNotFoundException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            context.Result = new JsonResult(new
            {
                error = new[] { context.Exception.Message },
                stackTrace = context.Exception.Demystify().StackTrace,
            });
        }
    }
}
