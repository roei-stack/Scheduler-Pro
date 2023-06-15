using System.ComponentModel.DataAnnotations;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace server.Models
{
    public class StaffFormInput
    {
        [Key]
        public string? StaffId { get; set; }

        public string? InstitutionUsername { get; set; }

        public string? UnavilableTimesJson { get; set; } 

        public List<Period>? GetUnavilableTimes()
        {
            return JsonSerializer.Deserialize<List<Period>>(UnavilableTimesJson);
        }

        public void SetUnavilableTimes(List<Period>? unavilableTimes)
        {
            UnavilableTimesJson = JsonSerializer.Serialize(unavilableTimes);
        }
    }
}
