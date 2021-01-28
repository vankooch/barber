namespace Barber.Core.Renderer
{
    using System.IO;
    using System.Threading.Tasks;
    using Scriban;

    public class ScribanRenderer : IRenderer
    {
        public async Task<string> Render(string template, object model)
        {
            var content = await File.ReadAllTextAsync(template);
            var ast = Template.Parse(content);
            var result = await ast.RenderAsync(model);

            return result;
        }
    }
}
