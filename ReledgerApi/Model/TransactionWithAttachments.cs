using System.Collections.Generic;

namespace ReledgerApi.Model
{
    public record TransactionWithAttachments : Transaction
    {
        public IEnumerable<Attachment> Attachments { get; init; }
    }
}
