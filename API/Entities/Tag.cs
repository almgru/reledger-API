namespace API.Entities
{
    public class Tag
    {
        public Tag(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}