using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class InstitutionData
    {
        public String? InstitutionName { get; set; }

        // must be initialized
        [Key]
        public String? Username { get; set; }

        public String? StaffFormId { get; set; }

        public String? StudentFormId { get; set; }
    }
}
