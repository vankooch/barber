namespace Barber.OpenApi.Settings
{
    public class I18nModel
    {
        public I18nModel()
        {
        }

        /// <summary>
        /// Destination path for generated files
        /// </summary>
        public string Destination { get; set; } = string.Empty;

        /// <summary>
        /// Language to use when requesting OpenApi specification
        /// </summary>
        public string Language { get; set; } = "en-US";

        /// <summary>
        /// Filename
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
