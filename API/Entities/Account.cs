using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace API.Entities
{
    public class Account
    {
        public enum IncreaseBalanceBehaviour
        {
            OnDebit, OnCredit
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Account.IncreaseBalanceBehaviour IncreaseBalanceOn { get; set; }

        public decimal Balance { get; set; }

        public IEnumerable<Account> ParentAccounts { get; set; } = Enumerable.Empty<Account>();
    }
}