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

        public Account Parent { get; set; }

        public void Debit(decimal amount)
        {
            if (IncreaseBalanceOn == IncreaseBalanceBehaviour.OnDebit)
            {
                Balance += amount;
            }
            else
            {
                Balance -= amount;
            }

            Parent?.Debit(amount);
        }

        public void Credit(decimal amount)
        {
            if (IncreaseBalanceOn == IncreaseBalanceBehaviour.OnCredit)
            {
                Balance += amount;
            }
            else
            {
                Balance -= amount;
            }

            Parent?.Credit(amount);
        }
    }
}