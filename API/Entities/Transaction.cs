using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using API.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Entities
{
    public class Transaction
    {
        public Transaction()
        {
            Tags = new List<Tag>();
            Attachments = new List<Attachment>();
        }

        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        public string Description { get; set; }

        [Required]
        [ModelBinder(typeof(AccountModelBinder))]
        public Account DebitAccount { get; set; }

        [Required]
        [ModelBinder(typeof(AccountModelBinder))]
        public Account CreditAccount { get; set; }

        public IEnumerable<Tag> Tags { get; set; }

        public IEnumerable<Attachment> Attachments { get; set; }
    }
}