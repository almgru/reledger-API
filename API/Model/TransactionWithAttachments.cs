using System.Collections.Generic;

namespace API.Model
{
    public record TransactionWithAttachments : Transaction
    {
        public IEnumerable<Attachment> Attachments { get; init; }
    }
}
