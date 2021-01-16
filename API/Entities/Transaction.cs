using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class Transaction
    {
        public Transaction(int id, decimal amount, string currency, DateTime date, string description,
                           Account debitAccount, Account creditAccount, IEnumerable<Tag> tags,
                           IEnumerable<Attachment> attachments)
        {
            this.Id = id;
            this.Amount = amount;
            this.Currency = currency;
            this.Date = date;
            this.Description = description;
            this.DebitAccount = debitAccount;
            this.CreditAccount = creditAccount;
            this.Tags = tags;
            this.Attachments = attachments;
        }

        public int Id { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public DateTime Date { get; private set; }
        public string Description { get; private set; }
        public Account DebitAccount { get; private set; }
        public Account CreditAccount { get; private set; }
        public IEnumerable<Tag> Tags { get; private set; }
        public IEnumerable<Attachment> Attachments { get; private set; }
    }
}