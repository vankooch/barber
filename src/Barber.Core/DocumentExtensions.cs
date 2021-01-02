namespace Barber.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Barber.Core.Models;
    using Microsoft.OpenApi.Models;
    using Microsoft.OpenApi.Readers;

    public static class DocumentExtensions
    {
        public static async Task<OpenApiDocument> ReadDocument(string url, string language = "en-US", CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("You need to set a URL");
            }

            Stream stream;
            if (url.Substring(0, 4) == "http")
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Accept-Language", language);
                stream = await httpClient.GetStreamAsync(new Uri(url), cancellationToken);
            }
            else
            {
                if (Path.IsPathRooted(url))
                {
                    url = Path.Combine(Directory.GetCurrentDirectory(), url);
                }

                stream = File.OpenRead(url);
            }

            // Read V3
            var document = new OpenApiStreamReader().Read(stream, out _);
            stream.Dispose();

            return document;
        }

        public static List<SchemaModel> ReadSchemas(this OpenApiDocument openApiDocument)
        {
            var result = new List<SchemaModel>();
            if (openApiDocument == null
                || openApiDocument.Components == null
                || openApiDocument.Components.Schemas == null
                || openApiDocument.Components.Schemas.Count == 0)
            {
                return result;
            }

            foreach (var openapiSchema in openApiDocument.Components.Schemas)
            {
                var resultSchema = new SchemaModel()
                {
                    Name = openapiSchema.Key,
                    Key = openapiSchema.Key,
                    Schema = openapiSchema.Value,
                };

                if (openapiSchema.Value.Properties != null
                    && openapiSchema.Value.Properties.Count > 0)
                {
                    foreach (var openapiProperty in openapiSchema.Value.Properties)
                    {
                        var resultProperty = new PropertyModel()
                        {
                            Key = openapiProperty.Key,
                            Name = openapiProperty.Key,
                            Description = openapiProperty.Value.Description,
                            IsNullable = openapiProperty.Value.Nullable,
                            IsRequired = openapiSchema.Value.Required?.Any(e => e == openapiProperty.Key) ?? false,
                            Schema = openapiProperty.Value,
                        };

                        if (openapiProperty.Value.Reference != null)
                        {
                            resultProperty.TypeReference = openapiProperty.Value.Reference.ReferenceV3;
                        }
                        else if (openapiProperty.Value.Items?.Reference != null)
                        {
                            resultProperty.TypeReference = openapiProperty.Value.Items.Reference.ReferenceV3;
                        }
                        else
                        {
                            resultProperty.Type = openapiProperty.Value.Type;
                        }

                        resultSchema.Properties.Add(resultProperty);
                    }
                }

                result.Add(resultSchema);
            }

            return result;
        }
    }
}
