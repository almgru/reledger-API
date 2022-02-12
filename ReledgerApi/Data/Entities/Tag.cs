using System.ComponentModel.DataAnnotations;

namespace ReledgerApi.Data.Entities
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
