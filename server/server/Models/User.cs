using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class User
    {
        [Key]
        public String? Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String? Password { get; set; }

        [Required]
        public bool IsInstitution { get; set; }
    }
}
