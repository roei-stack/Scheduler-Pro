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
        public int LecturePoints { get; set; }
        public int TAPoints { get; set; }
        // map(semester) -> occurrences
        public Dictionary<string, int> LectureOccurrences { get; set; }
        public Dictionary<string, int> TAOccurrences { get; set; }
        public int TAsAfterLecture { get; set; }
        public int LectureParts { get; set; }
        public bool IsYearlyClass { get; set; }

        // not input
        public List<UniStaff> CourseStaff { get; set; }
        // not input
        private HashSet<Course> NonOverlappingCourses { get; set; }
        // not input
        private Dictionary<Period, List<int>> NonAvailableStudants { get; set; }
        private int studentCounter;
        public int tlo;
        // map(semster) -> map(course group) -> part that left
        private Dictionary<string, Dictionary<int, int>> progressTableLeactures;
        private Dictionary<string, Dictionary<int, int>> progressTableTAs;

        public Course(int lecturePoints, int TAPoints, Dictionary<string, int> lectureOccurrences,
            Dictionary<string, int> TAOccurrences, int TAsAfterLecture,
            int LectureParts, bool isYearlyClass)
        {
            LecturePoints = lecturePoints;
            this.TAPoints = TAPoints;
            this.TAOccurrences = TAOccurrences;
            this.TAsAfterLecture = TAsAfterLecture;
            this.LectureParts = LectureParts;
            LectureOccurrences = lectureOccurrences;
            IsYearlyClass = isYearlyClass;

            studentCounter = 0;
            CourseStaff = new List<UniStaff>();
            NonOverlappingCourses = new HashSet<Course>();
            tlo = TotalLectureOccurrences();

            NonAvailableStudants = new Dictionary<Period, List<int>>();
            InitNonAvailableStudants();

            progressTableLeactures = new Dictionary<string, Dictionary<int, int>>();
            progressTableTAs = new Dictionary<string, Dictionary<int, int>>();
            InitProgressTables();
            IsYearlyClass = isYearlyClass;
        }

        private void InitNonAvailableStudants()
        {
            foreach (var semester in Constants.Semesters)
            {
                // Initialize with one-hour periods for each hour in the week
                for (int day = 1; day <= 6; day++)
                {
                    for (int hour = 8; hour < 22; hour++)
                    {
                        var startTime = hour;
                        var endTime = hour + 1;

                        var period = new Period(day, semester, startTime, endTime);
                        var whatStudantsWant = new List<int>();
                        for (int i = 0; i < tlo; i++)
                        {
                            whatStudantsWant.Add(0);
                        }
                        NonAvailableStudants.Add(period, whatStudantsWant);
                    }
                }
            }
        }

        private void InitProgressTables()
        {
            foreach (var semester in Constants.Semesters)
            {
                progressTableLeactures[semester] = new Dictionary<int, int>();
                for (int i = 1; i <= LectureOccurrences[semester]; i++)
                {
                    progressTableLeactures[semester][i] = LectureParts;
                }
                progressTableTAs[semester] = new Dictionary<int, int>();
                for (int i = 1; i <= TAOccurrences[semester]; i++)
                {
                    progressTableTAs[semester][i] = 1;
                }
            }
        }

        public override string ToString()
        {
            return $"General Course Number: {CourseId}\nLecture Occurrences: {LectureOccurrences}\nTA Occurrences: {TAOccurrences}\nTAs After Lecture: {TAsAfterLecture}\nLecture Parts: {LectureParts}\nCourse Staff: {string.Join(", ", CourseStaff)}\nNon-Overlapping Courses: {string.Join(", ", NonOverlappingCourses)}";
        }

        public int CreditPoints()
        {
            return LecturePoints + TAPoints;
        }

        public void UpdateStudentProblems(Student student)
        {
            foreach (Period period in student.UnavailableTimes)
            {
                NonAvailableStudants[period][studentCounter]++;
            }
            studentCounter++;
            studentCounter %= tlo;
        }

        public void UpdateOverlappingCourses(Major major)
        {
            int year = major.InWhatYearIncluded(this);
            if (-1 == year)
            {
                return;
            }
            foreach (var bundle in major.Bundles[year])
            {

                NonOverlappingCourses.UnionWith(bundle.SampleCourses());
            }
        }

        public void CleanThisFromOverlappingCourses()
        {
            NonOverlappingCourses.Remove(this);
        }

        private int TotalLectureOccurrences()
        {
            int sum = 0;
            foreach (var semester in Constants.Semesters)
            {
                sum += LectureOccurrences[semester];
            }
            return sum;
        }

        public void UpdateCourseStaff(UniStaff employee)
        {
            CourseStaff.Add(employee);
        }

        public UniStaff? FindStaff(Period period, string role)
        {
            Period realPeriod = new Period
            (
                period.Day,
                period.Semester,
                period.StartTime,
                period.EndTime + (LecturePoints / LectureParts) - 1
            );
            foreach (var employee in CourseStaff)
            {
                if (employee.IsSomeRole(this, role))
                {
                    if (employee.IsPeriodAvailable(realPeriod))
                    {
                        return employee;
                    }
                }
            }
            return null;
        }

        public bool IsOverlapping(Period period, Dictionary<Course, CourseScheduling> SuperCourses)
        {
            return false;
            foreach (var course in NonOverlappingCourses)
            {

            }
        }
    }
}
