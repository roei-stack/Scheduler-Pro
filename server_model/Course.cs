using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class Course
    {
        public int GeneralCourseNumber { get; set; }
        public string? CourseName { get; set; }
        public int LeacturePoints { get; set; }
        public int TAPoints { get; set; }
        public int LectureOccurrences { get; set; }
        public int TAOccurrences { get; set; }
        public int TAsAfterLecture { get; set; }
        public int LectureParts { get; set; }
        public List<UniStaff> CourseStaff { get; set; }
        public List<Course> NonOverlappingCourses { get; set; }
        public List<(Period, List<int>)> NonAvailableStudants { get; set; }

        public Course()
        {
            CourseStaff = new List<UniStaff>();
            NonOverlappingCourses = new List<Course>();
            NonAvailableStudants = new List<(Period, List<int>)>();
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
                    NonAvailableStudants.Add((period, whatStudantsWant));
                }
            }
        }

        public override string ToString()
        {
            return $"General Course Number: {GeneralCourseNumber}\nLecture Occurrences: {LectureOccurrences}\nTA Occurrences: {TAOccurrences}\nTAs After Lecture: {TAsAfterLecture}\nLecture Parts: {LectureParts}\nCourse Staff: {string.Join(", ", CourseStaff)}\nNon-Overlapping Courses: {string.Join(", ", NonOverlappingCourses)}";
        }


    }
}
