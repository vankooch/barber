namespace Barber.Core.Renderer
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Stubble.Core.Builders;

    public class Mustache : IRenderer
    {
        public async Task<string> Render(string template, object model)
        {
            var stubble = new StubbleBuilder().Build();
            using (var streamReader = new StreamReader(template, Encoding.UTF8))
            {
                var content = await streamReader.ReadToEndAsync();
                var output = await stubble.RenderAsync(content, model);

                return output;
            }
        }
    }
}
