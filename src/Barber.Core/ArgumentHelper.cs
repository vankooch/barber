namespace Barber.Core
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    public static class ArgumentHelper
    {
        public static object Parse(IEnumerable<string> args)
        {
            if (args == null || !args.Any())
            {
                return null;
            }

            dynamic model = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)model;
            foreach (var item in args)
            {
                var parts = item
                    .TrimStart('-')
                    .TrimStart('-')
                    .Split('=');

                if (parts.Length == 2)
                {
                    dictionary.Add(parts[0].Trim(), parts[1].Trim());
                }
            }

            return model;
        }
    }
}
