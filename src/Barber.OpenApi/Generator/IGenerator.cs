namespace Barber.OpenApi.Generator
{
    /// <summary>
    /// Generator declaration interface
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Generator name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Generator Type
        /// </summary>
        GeneratorType Type { get; }
    }
}
