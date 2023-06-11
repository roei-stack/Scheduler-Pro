using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class Course
    {
        public string? CourseId { get; set; }
        public string? CourseName { get; set; }
        public int LeacturePoints { get; set; }
        public int TAPoints { get; set; }
        public int LectureOccurrences { get; set; }
        public int TAOccurrences { get; set; }
        public int TAsAfterLecture { get; set; }
        public int LectureParts { get; set; }
        public (int, string?, int) YearSemesterLectureOccurrences { get; set; }
        public (int, string?, int) YearSemesterTAOccurrences { get; set; }
        public string? Semester { get; set; }
        public int Year { get; set; }
        // depend on you, but we have to agree on one thing
        public List<UniStaff> CourseStaff { get; set; }
        // not input
        private HashSet<Course> NonOverlappingCourses { get; set; }
        // not input
        private Dictionary<Period, List<int>> NonAvailableStudants { get; set; }
        private int studentCounter;
        private Random theSeed;

        public Course()
        {
            studentCounter = 0;
            theSeed = new Random();
            CourseStaff = new List<UniStaff>();
            NonOverlappingCourses = new HashSet<Course>();
            NonAvailableStudants = new Dictionary<Period, List<int>>();
            // Initialize with one-hour periods for each hour in the week
            for (int day = 1; day <= 6; day++)
            {
                for (int hour = 8; hour < 22; hour++)
                {
                    var startTime = TimeSpan.FromHours(hour);
                    var endTime = TimeSpan.FromHours(hour + 1);

                    var period = new Period { Day = day, StartTime = startTime, EndTime = endTime };
                    var whatStudantsWant = new List<int>();
                    for (int i = 0; i < LectureOccurrences; i++)
                    {
                        whatStudantsWant.Add(0);
                    }
                    NonAvailableStudants.Add(period, whatStudantsWant);
                }
            }
        }

        public override string ToString()
        {
            return $"General Course Number: {CourseId}\nLecture Occurrences: {LectureOccurrences}\nTA Occurrences: {TAOccurrences}\nTAs After Lecture: {TAsAfterLecture}\nLecture Parts: {LectureParts}\nCourse Staff: {string.Join(", ", CourseStaff)}\nNon-Overlapping Courses: {string.Join(", ", NonOverlappingCourses)}";
        }

        public int CreditPoints()
        {
            return LeacturePoints + TAPoints;
        }

        public void UpdateStudentProblems(Student student)
        {
            foreach (Period period in student.UnavailableTimes)
            {
                NonAvailableStudants[period][studentCounter]++;
            }
            studentCounter++;
            studentCounter %= LectureOccurrences;
        }

        public void UpdateOverlappingCourses(Major major)
        {
            if (!major.IsCourseIncluded(this))
            {
                return;
            }
            foreach (var bundle in major.Bundles[(Year, Semester)])
            {
                foreach (var course in bundle.Courses)
                {
                    NonOverlappingCourses.Add(course);
                }
            }
        }
    }
}
