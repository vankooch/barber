namespace Barber.OpenApi.Settings
{
    using System;
    using System.Collections.Generic;

    public class SchemaItemModel
    {
        public SchemaItemModel()
        {
        }

        /// <summary>
        /// Schema Name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Properties which are filtered out
        /// </summary>
        public IReadOnlyList<string> SkipProperties { get; set; } = Array.Empty<string>();
    }
}
