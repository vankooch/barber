namespace Barber.OpenApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Barber.Core;
    using Barber.OpenApi.Generator;
    using Barber.OpenApi.Models;
    using Barber.OpenApi.Models.Template;
    using Microsoft.OpenApi.Models;
    using Microsoft.OpenApi.Readers;

    /// <summary>
    /// Main processor for code generation
    /// </summary>
    public class Processor
    {
        private readonly IGeneratorTypes _generator;

        /// <summary>
        /// Constructor
        /// </summary>
        public Processor(IGeneratorTypes generator)
        {
            this._generator = generator;
            this.Settings = new Settings.SettingsModel();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">Use settings</param>
        public Processor(IGeneratorTypes generator, Settings.SettingsModel settings)
            : this(generator)
        {
            this.Settings = settings;
        }

        /// <summary>
        /// Open API Model
        /// </summary>
        public OpenApiDocument Api { get; set; }

        /// <summary>
        /// Open API Diagnostic
        /// </summary>
        public OpenApiDiagnostic Diagnostic { get; set; }

        /// <summary>
        /// Generators
        /// </summary>
        public List<IGenerator> Generators { get; set; } = new List<IGenerator>();

        /// <summary>
        /// Settings
        /// </summary>
        public Settings.SettingsModel Settings { get; set; }

        /// <summary>
        /// Run convert for given step
        /// </summary>
        /// <param name="step">Step Model</param>
        public void Convert(Settings.StepModel step)
        {
            var generator = this.Generators.FirstOrDefault(e => e.Name == step.Generator);
            if (generator == null)
            {
                return;
            }

            PropertyModel[] filter(string name, PropertyModel[] list)
            {
                var filterResult = new PropertyModel[] { };
                if (this.Settings.SkipProperties?.Length > 0)
                {
                    filterResult = list.Where(e => !this.Settings.SkipProperties.Contains(e.Name)).ToArray();
                }

                var match = this.Settings.SchemaConfig?.FirstOrDefault(e => e.Name == name);
                if (match != null)
                {
                    filterResult = filterResult.Where(e => !match.SkipProperties.Contains(e.Name)).ToArray();
                }

                return filterResult;
            }

            var result = new Dictionary<string, IRenderModel>();
            switch (generator.Type)
            {
                case GeneratorType.Schema:
                    var schemas = this.ConvertSchemas(generator as IGeneratorSchema, step.Destination, filter, step.ItemsSkips, step.ItemsIncludes);
                    result = new Dictionary<string, IRenderModel>(schemas.Count);
                    foreach (var item in schemas)
                    {
                        result.Add(item.Key, item.Value as IRenderModel);
                    }

                    step.Result = result;
                    break;

                case GeneratorType.Path:
                    var paths = this.ConvertPaths(generator as IGeneratorPaths, step.Destination, step.ItemsSkips, step.ItemsIncludes);
                    result = new Dictionary<string, IRenderModel>(paths.Count);
                    foreach (var item in paths)
                    {
                        result.Add(item.Key, item.Value as IRenderModel);
                    }

                    step.Result = result;
                    break;

                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Reads all Paths from OpenAPI object and converts it to models used for templates.
        /// </summary>
        /// <param name="generator">Generator</param>
        /// <param name="outputPath">Output Path</param>
        /// <param name="skip">Skip List</param>
        /// <param name="include">Include List</param>
        /// <returns></returns>
        public IDictionary<string, ServiceModel> ConvertPaths(
            IGeneratorPaths generator,
            string outputPath,
            IEnumerable<string> skip = null,
            IEnumerable<string> include = null)
        {
            if (this.Api == null
                || this.Api.Paths == null
                || this.Api.Paths.Count == 0)
            {
                return null;
            }

            var dict = new Dictionary<string, ServiceModel>();
            var paths = new List<PathModel>();

            foreach (var path in this.Api.Paths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    var references = new List<ReferenceModel>();
                    var pathModel = generator.GetPath(path, operation);
                    if (pathModel == null)
                    {
                        continue;
                    }

                    // Parameters
                    if (operation.Value.Parameters?.Count > 0)
                    {
                        var parameters = new List<PropertyModel>();
                        foreach (var item in operation.Value.Parameters)
                        {
                            if (this.Settings.HasApiVersion && item.Name == "version" && item.Schema.Type == "string")
                            {
                                continue;
                            }

                            var add = generator.GetParameter(item);
                            if (add != null)
                            {
                                pathModel.AddReference(add.Reference);
                                parameters.Add(add);
                            }
                        }

                        pathModel.Parameters = parameters.ToArray();
                    }

                    // Body
                    if (operation.Value.RequestBody != null)
                    {
                        var requestBody = operation.Value.RequestBody.Content.FirstOrDefault(e => e.Key == this.Settings.BodyMediaType);
                        if (string.IsNullOrEmpty(requestBody.Key))
                        {
                            requestBody = operation.Value.RequestBody.Content.FirstOrDefault();
                        }

                        if (!string.IsNullOrEmpty(requestBody.Key))
                        {
                            pathModel.AddReference(this._generator.GetReference(requestBody.Value?.Schema));
                            pathModel.Body = generator.GetMediaType(requestBody);
                        }
                    }

                    // Response Type
                    if (operation.Value.Responses != null)
                    {
                        // Try match in order
                        foreach (var item in this.Settings.ResponseCodes)
                        {
                            var response = operation.Value.Responses
                                .FirstOrDefault(e => e.Key == item);
                            if (!string.IsNullOrEmpty(response.Key))
                            {
                                var requestBody = response.Value.Content.FirstOrDefault(e => e.Key == this.Settings.ResponseMediaType);
                                if (string.IsNullOrEmpty(requestBody.Key))
                                {
                                    requestBody = response.Value.Content.FirstOrDefault();
                                }

                                if (!string.IsNullOrEmpty(requestBody.Key))
                                {
                                    pathModel.AddReference(this._generator.GetReference(requestBody.Value?.Schema));
                                    pathModel.ReturnType = generator.GetMediaType(requestBody);
                                }

                                break;
                            }
                        }
                    }

                    paths.Add(pathModel);
                }
            }

            // Group
            var tags = paths
                .SelectMany(e => e.Tags)
                .Distinct();
            foreach (var tag in tags)
            {
                var add = generator.GetService(tag, paths);
                if (add != null)
                {
                    // Global skip
                    if (this.Settings.SkipTags != null && this.Settings.SkipTags.Any(e => e == tag))
                    {
                        continue;
                    }

                    // Step Skip
                    if (skip != null && skip.Any(e => e == tag))
                    {
                        continue;
                    }

                    // Step Include
                    if (include != null && !include.Any(e => e == tag))
                    {
                        continue;
                    }

                    // Set reference
                    if (add.Paths?.Length > 0)
                    {
                        var refs = add.Paths
                            .Where(e => e.References?.Length > 0)
                            .SelectMany(e => e.References);
                        if (refs != null && refs?.Count() > 0)
                        {
                            add.References = this.CleanReferences(refs);
                        }
                    }

                    // Set Path
                    add.File.Path = this.GetOutputPath(outputPath, add.File.Name);

                    dict.Add(tag, add);
                }
            }

            return dict;
        }

        /// <summary>
        /// Reads all Schema's from OpenAPI object and converts them.
        /// </summary>
        /// <param name="generator">Generator</param>
        /// <param name="outputPath">Output Path</param>
        /// <param name="propertyFilter">Property Filter</param>
        /// <param name="skip">Skip List</param>
        /// <param name="include">Include List</param>
        /// <returns></returns>
        public IDictionary<string, SchemaModel> ConvertSchemas(
            IGeneratorSchema generator,
            string outputPath,
            Func<string, PropertyModel[], PropertyModel[]> propertyFilter,
            IEnumerable<string> skip = null,
            IEnumerable<string> include = null)
        {
            if (this.Api == null
                || this.Api.Components == null
                || this.Api.Components.Schemas == null
                || this.Api.Components.Schemas.Count == 0)
            {
                return null;
            }

            var list = new Dictionary<string, SchemaModel>();
            foreach (var item in this.Api.Components.Schemas)
            {
                // Global skip
                if (this.Settings.SkipSchemas != null && this.Settings.SkipSchemas.Any(e => e == item.Key))
                {
                    continue;
                }

                // Step Skip
                if (skip != null && skip.Any(e => e == item.Key))
                {
                    continue;
                }

                // Step Include
                if (include != null && !include.Any(e => e == item.Key))
                {
                    continue;
                }

                var schema = generator.GetSchema(item);
                if (schema == null)
                {
                    continue;
                }

                schema.Schema = item.Value;
                if (item.Value.Properties?.Count > 0)
                {
                    var props = new List<PropertyModel>();
                    foreach (var property in item.Value.Properties)
                    {
                        var add = generator.GetProperty(item, property);
                        if (add != null)
                        {
                            props.Add(add);
                            schema.AddReference(add.Reference, true);
                        }
                    }

                    schema.Properties = props.ToArray();
                    if (propertyFilter != null)
                    {
                        schema.PropertiesFiltered = propertyFilter(schema.Name, schema.Properties);
                    }
                }

                // Set Path
                schema.File.Path = this.GetOutputPath(outputPath, schema.File.Name);

                list.Add(item.Key, schema);
            }

            return list;
        }

        /// <summary>
        /// Read OpenAPI JSON from URL / File
        /// </summary>
        /// <param name="url">URL / File path</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public async Task Read(string url, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("You need to set a URL");
            }

            Stream stream;
            if (url.Substring(0, 4) == "http")
            {
                var httpClient = new HttpClient();
                stream = await httpClient.GetStreamAsync(new Uri(url));
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
            this.Api = new OpenApiStreamReader().Read(stream, out var diagnostic);
            this.Diagnostic = diagnostic;

            stream.Dispose();
        }

        /// <summary>
        /// Render Templates
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="schemaModels">List of Models</param>
        /// <param name="renderer">Renderer to use</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public async Task<IDictionary<string, T>> Render<T>(
            IDictionary<string, T> schemaModels,
            IRenderer renderer = null,
            CancellationToken cancellationToken = default)
            where T : IRenderModel
        {
            if (schemaModels == null || schemaModels.Count() == 0)
            {
                return schemaModels;
            }

            if (renderer == null)
            {
                renderer = new Core.Renderer.Mustache();
            }

            foreach (var item in schemaModels)
            {
                var template = this.GetTemplateFullPath(this.Settings.TemplateRoot, item.Value.File.Template);
                if (item.Value.File != null)
                {
                    item.Value.File.Content = await renderer.Render(template, item.Value);
                }
            }

            return schemaModels;
        }

        /// <summary>
        /// Update references
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            if (this.Settings.Steps == null || this.Settings.Steps.Length == 0)
            {
                return;
            }

            var global = this.Settings.Steps
                .SelectMany(e => e.Result)
                .ToArray();

            foreach (var step in this.Settings.Steps)
            {
                if (step.Result == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(step.Resolve) && !string.IsNullOrWhiteSpace(step.Resolve))
                {
                    var resolver = global;
                    if (step.Resolve.ToLower() != "global")
                    {
                        var match = this.Settings.Steps.FirstOrDefault(e => e.Name == step.Resolve);
                        if (match != null)
                        {
                            resolver = match.Result.ToArray();
                        }
                    }

                    this.UpdateReferences(step.Result, resolver);
                }

                foreach (var file in step.Result)
                {
                    file.Value.File.Template = step.Template;
                }

                step.Result = await this.Render(step.Result);
            }

            return;
        }

        /// <summary>
        /// Update references paths
        /// </summary>
        /// <typeparam name="TA">Model A</typeparam>
        /// <typeparam name="TB">Model B</typeparam>
        /// <param name="schemaModels">Dictionary to update</param>
        /// <param name="references">Legend dictionary for look up</param>
        public void UpdateReferences<TA, TB>(IDictionary<string, TA> schemaModels, KeyValuePair<string, TB>[] references)
            where TA : IRenderModel
            where TB : IRenderModel
        {
            if (schemaModels == null || schemaModels.Count() == 0
                || references == null || references.Count() == 0)
            {
                return;
            }

            foreach (var item in schemaModels)
            {
                if (item.Value.File == null || item.Value.References == null || item.Value.References.Length == 0)
                {
                    continue;
                }

                foreach (var reference in item.Value.References)
                {
                    var match = references
                        .Where(e => e.Key == reference.Key)
                        .Select(e => e.Value)
                        .FirstOrDefault();
                    if (match != null)
                    {
                        reference.Name = match.Name;
                        reference.File = this._generator.GetReferencePath(this.GetPathDiff(item.Value.File.Path, match.File.Path));
                    }
                }
            }
        }

        /// <summary>
        /// Write files
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="schemaModels">List of Models</param>
        public void Write<T>(IDictionary<string, T> schemaModels)
            where T : IRenderModel
        {
            if (schemaModels == null || schemaModels.Count() == 0)
            {
                return;
            }

            foreach (var item in schemaModels)
            {
                if (item.Value.File == null)
                {
                    continue;
                }

                var dir = Path.GetDirectoryName(item.Value.File.Path);
                if (!Directory.Exists(item.Value.File.Path))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(item.Value.File.Path, item.Value.File.Content);
            }
        }

        private ReferenceModel[] CleanReferences(IEnumerable<ReferenceModel> data)
        {
            if (data == null || data.Count() == 0)
            {
                return new ReferenceModel[] { };
            }

            var list = new List<ReferenceModel>();
            foreach (var item in data)
            {
                if (list.Any(e => e.Key == item.Key))
                {
                    continue;
                }

                list.Add(item);
            }

            return list.ToArray();
        }

        private string GetOutputPath(string path, string file)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path) && !Path.IsPathRooted(path))
            {
                // For output we use current directory as root
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
            }

            return Path.Combine(path, file);
        }

        private string GetPathDiff(string source, string destination)
        {
            var sourceUri = new Uri(source);
            var destinationUri = new Uri(destination);

            var relative = sourceUri.MakeRelativeUri(destinationUri);

            return relative.OriginalString;
        }

        private string GetTemplateFullPath(string path, string file)
        {
            var final = string.Empty;
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path) && !Path.IsPathRooted(path))
            {
                // First check relative
                final = this.GetOutputPath(path, file);
                if (!File.Exists(final))
                {
                    // For templates we use assembly path as root
                    var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    path = Path.Combine(root, path);
                    final = Path.Combine(path, file);
                }
            }
            else
            {
                final = Path.Combine(path, file);
            }

            if (!File.Exists(final))
            {
                throw new Exception($"Could not find template file in: {final}");
            }

            return final;
        }
    }
}
