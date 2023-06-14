using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class InstitutionData
    {
        // must be initialized
        [Key]
        public String? Username { get; set; }

        public String? StaffFormId { get; set; }

        public String? StudentFormId { get; set; }

        public InstitutionInput? InstitutionInput { get; set; }
    }
}
