namespace server.Models
{
    [Serializable]
    public class ScheduledStudentCourseGroupData
    {
        public string? LessonType { get; set; }

        public int GroupNumber { get; set; }

        public string? CourseName { get; set; }
        public string? CourseId { get; set; }

        public bool IsEmpty { get; set; }
    }
}
