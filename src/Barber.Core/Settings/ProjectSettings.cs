namespace Barber.Core.Settings
{
    using System.Collections.Generic;
    using Barber.Core.Converter;

    public class ProjectSettings
    {
        public string? Api { get; set; }

        public List<ConverterOrderSettings> Presets { get; set; } = new List<ConverterOrderSettings>();

        public List<SchemaConvetSettings> SchemaJobs { get; set; } = new List<SchemaConvetSettings>();

        public static ProjectSettings Default() => new ProjectSettings()
        {
            Presets = new List<ConverterOrderSettings>()
                {
                    new ConverterOrderSettings()
                    {
                        Name = "Samples",
                        PropertyNameConverter = new List<ConverterParameterSettings>()
                        {
                            new ConverterParameterSettings()
                            {
                                Name = nameof(SuffixConverter),
                                Option = new SuffixConverter().GetSampleOptions(),
                                PropertyFilter = "Id",
                            },
                        },
                        PropertyTypeConverter = new List<ConverterParameterSettings>()
                        {
                            new ConverterParameterSettings()
                            {
                                Name = nameof(TypeConverter),
                                Option = new TypeConverter().GetSampleOptions(),
                            },
                            new ConverterParameterSettings()
                            {
                                Name = nameof(NullableConverter),
                                Option = new NullableConverter().GetSampleOptions(),
                            },
                        },
                        PropertyDefaultValueConverter = new List<ConverterParameterSettings>()
                        {
                            new ConverterParameterSettings()
                            {
                                Name = nameof(DefaultValueConverter),
                                Option = new DefaultValueConverter().GetSampleOptions(),
                            },
                        },
                        SchemaNameConverter = new List<ConverterParameterSettings>()
                        {
                            new ConverterParameterSettings()
                            {
                                Name = nameof(PrefixConverter),
                                Option = new PrefixConverter().GetSampleOptions(),
                            },
                            new ConverterParameterSettings()
                            {
                                Name = nameof(SuffixConverter),
                                Option = new SuffixConverter().GetSampleOptions(),
                                SchemaFilter = "UserModel",
                            },
                            new ConverterParameterSettings()
                            {
                                Name = nameof(ReplaceConverter),
                                Option = new ReplaceConverter().GetSampleOptions(),
                            },
                        },
                    },
                },
            SchemaJobs = new List<SchemaConvetSettings>()
                {
                    new SchemaConvetSettings()
                    {
                        Name = "SampleConvert",
                        Preset = "Samples",
                        IsSingleFile = true,
                    },
                },
        };
    }
}
