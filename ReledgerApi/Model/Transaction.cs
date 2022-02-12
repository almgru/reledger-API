using System;
using System.Collections.Generic;

namespace ReledgerApi.Model
{
    public record Transaction
    {
        public decimal Amount { get; init; }

        public string Currency { get; init; }

        public string DebitAccount { get; init; }

        public string CreditAccount { get; init; }

        public DateTime DateTime { get; init; }

        public string Description { get; init; }

        public IEnumerable<string> Tags { get; init; }
    }
}
