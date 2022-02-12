using System.ComponentModel.DataAnnotations;
using API.Constants;

namespace API.Data.Entities
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IncreaseBalanceBehavior IncreaseBalanceOn { get; set; }

        public decimal Balance { get; set; }

        public int? ParentId { get; set; }
    }
}
