using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Attachment
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}