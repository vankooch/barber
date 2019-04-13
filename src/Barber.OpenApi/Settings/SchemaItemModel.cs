namespace Barber.OpenApi.Settings
{
    public class SchemaItemModel
    {
        public SchemaItemModel()
        {
        }

        /// <summary>
        /// Schema Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Properties which are filtered out
        /// </summary>
        public string[] SkipProperties { get; set; }
    }
}
