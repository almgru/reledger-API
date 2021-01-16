using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public Account DebitAccount { get; set; }
        public Account CreditAccount { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
    }
}