namespace Barber.Core.Renderer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Barber.Core.Models;
    using Stubble.Core.Builders;
    using Stubble.Helpers;

    public class MustacheRenderer : IRenderer
    {
        public async Task<string> Render(string template, object model)
        {
            var helpers = new Helpers()
                .Register("ToUpper", (HelperContext context, string data) =>
                {
                    return data.ToUpperInvariant();
                })
                .Register("ToUpperFirst", (HelperContext context, string data) =>
                {
                    return data[0].ToString().ToUpperInvariant() + data[1..];
                })
                .Register("ToLower", (HelperContext context, string data) =>
                {
                    return data.ToLowerInvariant();
                })
                .Register("ToLowerFirst", (HelperContext context, string data) =>
                {
                    return data[0].ToString().ToLowerInvariant() + data[1..];
                })
                .Register("ListWithComma", (HelperContext context, IEnumerable<string> data) =>
                {
                    return string.Join(", ", data);
                })
                .Register("ListWithComma", (HelperContext context, IEnumerable<ReferencedSchemasItemModel> data) =>
                {
                    var list = data?.Select(e => e.Name);
                    return string.Join(", ", list);
                })
                .Register("HasProperty", (HelperContext context, string name) =>
                {
                    var data = context.Lookup<IEnumerable<PropertyModel>>("Properties");
                    if (data?.Any(e => e.Name == name) == true)
                    {
                        return true;
                    }

                    return false;
                });

            var stubble = new StubbleBuilder()
                .Configure(conf => conf.AddHelpers(helpers))
                .Build();

            using (var streamReader = new StreamReader(template, Encoding.UTF8))
            {
                var content = await streamReader.ReadToEndAsync();
                var output = await stubble.RenderAsync(content, model);

                return output;
            }
        }
    }
}
