using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Institution
    {
        [Key]
        public String? Name { get; set; }
    }
}
