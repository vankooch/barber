namespace Barber.Core.Settings
{
    using System.Collections.Generic;

    public class BlockPropertiesModel
    {
        public List<string> Properties { get; set; } = new List<string>();

        public string Schema { get; set; } = string.Empty;
    }
}
