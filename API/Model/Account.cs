namespace API.Model
{
    public record Account
    {
        public string Name { get; init; }

        public decimal Balance { get; init; }
    }
}
