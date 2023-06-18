using server.ViewModels;
using System.ComponentModel.DataAnnotations;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace server.Models
{
    public class StudentAlgoResult
    {
        // id == `username + '&' + name`
        [Key]
        public string? Id { get; set; }

        public string? Username { get; set; }

        public string? Name { get; set; }

        public string? ListResultJson { get; set; }

        public List<StudentAlgorithmResultItemViewModel> GetListResult()
        {
            return JsonSerializer.Deserialize<List<StudentAlgorithmResultItemViewModel>>(ListResultJson);
        }

        public void SetListResult(List<StudentAlgorithmResultItemViewModel> value)
        {
            ListResultJson = JsonSerializer.Serialize(value);
        }
    }
}
