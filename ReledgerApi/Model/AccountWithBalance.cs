namespace ReledgerApi.Model
{
    public record AccountWithBalance : Account
    {
        public decimal Balance { get; init; }
    }
}
