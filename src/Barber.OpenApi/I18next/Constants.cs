namespace Barber.OpenApi.I18next
{
    using System.Collections.Generic;

    public static class Constants
    {
        public const string CONTEXT_FEMALE = "Context_female";
        public const string CONTEXT_MALE = "Context_male";
        public const string INTERPOLATE = "Interpolate";
        public const string INTERPOLATE_UNESCAPED = "InterpolateUnescaped";
        public const string INTERPOLATE_WITH_FORMATTING = "InterpolateWithFormatting";
        public const string NESTING = "Nesting";
        public const string PLURAL_SIMPLE = "PluralSimple";
        public const string PLURAL_SIMPLE_PLURAL = "PluralSimple_plural";

        public static List<ItemModel> ConvertToItemGroup(IDictionary<string, string> data)
        {
            var items = new List<ItemModel>();
            var keys = new Dictionary<string, string>();

            if (data == null || data.Count == 0)
            {
                return items;
            }

            foreach (var item in data)
            {
                if (IsGroupItem(item.Key))
                {
                    continue;
                }

                keys.Add(item.Key, item.Value);
            }

            if (keys == null || keys.Count == 0)
            {
                return items;
            }

            foreach (var key in keys)
            {
                var itemgroup = new Dictionary<string, string>();
                foreach (var item in data)
                {
                    if (item.Key == key.Key || IsGroupItem(item.Key, key.Key))
                    {
                        itemgroup.Add(item.Key, item.Value);
                    }
                }

                if (itemgroup.Count > 0)
                {
                    items.Add(new ItemModel(key.Key, itemgroup));
                }
            }

            return items;
        }

        public static bool IsGroupItem(string key)
        {
            if (key.EndsWith(CONTEXT_FEMALE)
                    || key.EndsWith(CONTEXT_MALE)
                    || key.EndsWith(INTERPOLATE)
                    || key.EndsWith(INTERPOLATE_UNESCAPED)
                    || key.EndsWith(INTERPOLATE_WITH_FORMATTING)
                    || key.EndsWith(NESTING)
                    || key.EndsWith(PLURAL_SIMPLE)
                    || key.EndsWith(PLURAL_SIMPLE_PLURAL))
            {
                return true;
            }

            return false;
        }

        public static bool IsGroupItem(string key, string itemKey)
        {
            if (key == itemKey + CONTEXT_FEMALE
                    || key == itemKey + CONTEXT_MALE
                    || key == itemKey + INTERPOLATE
                    || key == itemKey + INTERPOLATE_UNESCAPED
                    || key == itemKey + INTERPOLATE_WITH_FORMATTING
                    || key == itemKey + NESTING
                    || key == itemKey + PLURAL_SIMPLE
                    || key == itemKey + PLURAL_SIMPLE_PLURAL)
            {
                return true;
            }

            return false;
        }
    }
}
