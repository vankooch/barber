namespace Barber.IoT.Api.Models
{
    using Microsoft.Extensions.Options;

    public class OptionModel<TOptions> : IOptions<TOptions>
        where TOptions : class, new()
    {
        public OptionModel()
        {
        }

        public OptionModel(TOptions value)
            => this.Value = value;

        public TOptions Value { get; set; }
    }
}
