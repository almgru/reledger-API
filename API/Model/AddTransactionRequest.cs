using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using API.Converters;

namespace API.Model
{
    public record AddTransactionRequest
    {
        [Required]
        [JsonConverter(typeof(JsonStringDecimalConverter))]
        public decimal Amount { get; init; }

        [Required]
        public string Currency { get; init; }

        [Required]
        public string DebitAccount { get; init; }

        [Required]
        public string CreditAccount { get; init; }

        public DateTime DateTime { get; init; } = DateTime.UtcNow;

        public string Description { get; init; }

        public IEnumerable<string> Tags { get; init; }

        public IEnumerable<Attachment> Attachments { get; init; }
    }
}
