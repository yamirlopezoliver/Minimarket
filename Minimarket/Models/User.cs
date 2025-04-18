using System;
using System.ComponentModel.DataAnnotations;

namespace Minimarket.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

      
        

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}