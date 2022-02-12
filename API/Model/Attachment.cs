using System.ComponentModel.DataAnnotations;

namespace API.Model
{
    public record Attachment
    {
        [Required]
        public string Name { get; init; }

        [Required]
        public byte[] Data { get; init; }
    }
}
