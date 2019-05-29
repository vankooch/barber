namespace Barber.Core.Renderer
{
    using System.Threading.Tasks;

    public interface IRenderer
    {
        Task<string> Render(string template, object model);
    }
}
