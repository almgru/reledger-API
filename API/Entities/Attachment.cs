namespace API.Entities
{
    public class Attachment
    {
        public Attachment(string name, byte[] data)
        {
            this.Name = name;
            this.Data = data;
        }

        public string Name { get; private set; }
        public byte[] Data { get; private set; }
    }
}