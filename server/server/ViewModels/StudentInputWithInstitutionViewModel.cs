using CourseModel;

namespace server.ViewModels
{
    public class StudentInputWithInstitutionViewModel
    {
        public string? Name { get; set; }

        public string? InstitutionName { get; set; }

        public int LearningDays { get; set; }
        public int HoursPerDay { get; set; }
        public bool SemesterA { get; set; }
        public bool SemesterB { get; set; }
        public bool SemesterSummer { get; set; }
        public List<Period>? UnavilableTimes { get; set; }
        public List<string>? CourseIds { get; set; }
    }
}
