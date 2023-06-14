namespace server.Models
{
    public class InstitutionInput
    {
        public string? InstitutionName { get; set; }
        public List<Course> CourseList { get; set; }
        public List<UniStaff> Staff { get; set; }
        public List<Major> Majors { get; set; }
        public List<Student> Students { get; set; }
        public InstitutionInput(string? institutionName, List<Course> courseList,
            List<UniStaff> staff, List<Student> students, List<Major> majors)
        {
            InstitutionName = institutionName;
            CourseList = courseList;
            Staff = staff;
            Students = students;
            Majors = majors;
        }
    }
}
