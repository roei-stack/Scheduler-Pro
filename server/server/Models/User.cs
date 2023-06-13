using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public bool NeedSetup { get; set; }
    }
}
