using System.ComponentModel.DataAnnotations;

namespace API.Model
{
    public record AddAccountRequest
    {
        [Required]
        public string Name { get; init; }
    }
}
