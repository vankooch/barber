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

        public string Key { get; set; }
    }
}
