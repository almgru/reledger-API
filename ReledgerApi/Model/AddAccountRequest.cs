using System.ComponentModel.DataAnnotations;

namespace ReledgerApi.Model
{
    public record AddAccountRequest
    {
        [Required]
        public string Name { get; init; }
    }
}
