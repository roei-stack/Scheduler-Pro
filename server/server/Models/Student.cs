namespace server.Models
{
    public class Student
    {
        public string? Name { get; set; }
        public int? Id { get; set; }
        // student's input
        public List<Period> UnavailableTimes { get; set; }
        // student's input (or the institution if default for major)
        public List<Course> WantedCourses { get; set; }

        public Student(List<Period> unavailableTimes, List<Course> wantedCourses)
        {
            UnavailableTimes = unavailableTimes;
            WantedCourses = wantedCourses;
        }

        public Student()
        {
            UnavailableTimes = new List<Period>();
            WantedCourses = new List<Course>();
        }
    }
}
