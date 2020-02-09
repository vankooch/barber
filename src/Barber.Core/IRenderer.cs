namespace Barber.Core
{
    using System.Threading.Tasks;

    public interface IRenderer
    {
        Task<string> Render(string templateString, object model);
    }
}
