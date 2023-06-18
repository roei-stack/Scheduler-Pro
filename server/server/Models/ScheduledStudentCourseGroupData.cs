namespace server.Models
{
    public class ScheduledStudentCourseGroupData
    {
        public string? lessonType { get; set; }

        public int groupNumber { get; set; }

        public string? courseName { get; set; }
        public string? courseId { get; set; }

        public bool isEmpty { get; set; }
    }
}
