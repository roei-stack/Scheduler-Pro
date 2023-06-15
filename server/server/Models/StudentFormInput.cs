using CourseModel;
using System.ComponentModel.DataAnnotations;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace server.Models
{
    public class StudentFormInput
    {
        [Key]
        public string? StudentId { get; set; }

        public string? InstitutionUsername { get; set; }

        public string? UnavilableTimesJson { get; set; }

        public string? CourseIdsJson { get; set; }

        public List<Period>? GetUnavilableTimes()
        {
            return JsonSerializer.Deserialize<List<Period>>(UnavilableTimesJson);
        }

        public void SetUnavilableTimes(List<Period>? unavilableTimes)
        {
            UnavilableTimesJson = JsonSerializer.Serialize(unavilableTimes);
        }

        public List<string>? GetCourseIds()
        {
            return JsonSerializer.Deserialize<List<string>>(CourseIdsJson);
        }

        public void SetCourseIds(List<string>? courseIds)
        {
            CourseIdsJson = JsonSerializer.Serialize(courseIds);
        }
    }
}
