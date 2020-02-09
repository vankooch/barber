#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Barber.Cli.Models
{
    public class ItemDataModel<T>
    {
        public ItemDataModel()
        {
        }

        public ItemDataModel(string key, T data)
        {
            this.Key = key;
            this.Data = data;
        }

        public T Data { get; set; }

        public string? Key { get; set; }

        public string? Title { get; set; }
    }
}
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
