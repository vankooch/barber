namespace Barber.OpenApi.Settings
{
    using System.Collections.Generic;

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
        public IReadOnlyList<string> SkipProperties { get; set; }
    }
}
