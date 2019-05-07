namespace Barber.OpenApi.I18next
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class ItemModel
    {
        private readonly IDictionary<string, string> _data = new Dictionary<string, string>();

        public ItemModel(string name, IDictionary<string, string> data)
        {
            this.Name = name;
            this._data = data;
        }

        public ItemModel(string key, string value)
        {
            this.Name = key;
            this._data.Add(key, value);
        }

        public string ContextFemale
        {
            get => this.GetSuffixValue(Constants.CONTEXT_FEMALE);
            set => this.SetSuffixValue(Constants.CONTEXT_FEMALE, value);
        }

        public string ContextMale
        {
            get => this.GetSuffixValue(Constants.CONTEXT_MALE);
            set => this.SetSuffixValue(Constants.CONTEXT_MALE, value);
        }

        public string Interpolate
        {
            get => this.GetSuffixValue(Constants.INTERPOLATE);
            set => this.SetSuffixValue(Constants.INTERPOLATE, value);
        }

        public string InterpolateUnescaped
        {
            get => this.GetSuffixValue(Constants.INTERPOLATE_UNESCAPED);
            set => this.SetSuffixValue(Constants.INTERPOLATE_UNESCAPED, value);
        }

        public string InterpolateWithFormatting
        {
            get => this.GetSuffixValue(Constants.INTERPOLATE_WITH_FORMATTING);
            set => this.SetSuffixValue(Constants.INTERPOLATE_WITH_FORMATTING, value);
        }

        public string Name { get; private set; }

        public string Nesting
        {
            get => this.GetSuffixValue(Constants.NESTING);
            set => this.SetSuffixValue(Constants.NESTING, value);
        }

        public string PluralSimple
        {
            get => this.GetSuffixValue(Constants.PLURAL_SIMPLE);
            set => this.SetSuffixValue(Constants.PLURAL_SIMPLE, value);
        }

        public string PluralSimplePlural
        {
            get => this.GetSuffixValue(Constants.PLURAL_SIMPLE_PLURAL);
            set => this.SetSuffixValue(Constants.PLURAL_SIMPLE_PLURAL, value);
        }

        public IDictionary<string, string> Data => this._data;

        public string GetSuffixValue(string suffix)
        {
            if (this._data.TryGetValue(this.Name + suffix, out var match))
            {
                return match;
            }

            return string.Empty;
        }

        public void SetSuffixValue(string suffix, string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            this._data.Add(this.Name + suffix, value);
        }
    }
}
