using System.ComponentModel.DataAnnotations;
using API.Constants;

namespace API.Model
{
    public record AddAccountRequest
    {
        [Required]
        public string Name { get; init; }

        public IncreaseBalanceBehavior IncreaseBalanceOn { get; init; } =
            IncreaseBalanceBehavior.OnDebit;

        public decimal Balance { get; init; } = 0.0m;
    }
}
