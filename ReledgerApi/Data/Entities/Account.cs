using System.ComponentModel.DataAnnotations;

namespace ReledgerApi.Data.Entities
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? ParentId { get; set; }
    }
}
