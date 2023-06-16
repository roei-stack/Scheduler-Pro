using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        // not input
        public List<UniStaff> CourseStaff { get; set; }
        // not input
        private Dictionary<Period, List<int>> NonAvailableStudants { get; set; }
        // for using to what lecture group
        private int studentCounter;
        // total occurrences
        public int tLo;
        public int tTAo;
        public int currentGroupNumber = 1;
        /*
        // map(semster) -> map(course group) -> part that left
        private Dictionary<string, Dictionary<int, int>> progressTableLeactures;
        private Dictionary<string, Dictionary<int, int>> progressTableTAs;*/
        // map(semester) -> map(phase) -> UnoverlapableTimes / NonOverlappingCourses
        public Dictionary<string, Dictionary<int, List<Period>>> TheUnoverlapableTimes;
        private Dictionary<string, Dictionary<int, HashSet<Course>>> NonOverlappingCourses { get; set; }
        // how many students are in this course
        private int studentNumber;
        // map(semester) -> max phase
        public static Dictionary<string, int> MaxPhase;

        static Course()
        {
            MaxPhase = new Dictionary<string, int>();
            foreach (string semester in Constants.Semesters)
            {
                MaxPhase[semester] = 0;
            }
        }

        public Course(string CourseId, string CourseName, int LecturePoints, int TAPoints,
                Dictionary<string, int> LectureOccurrences,
                Dictionary<string, int> TAOccurrences, int TAsAfterLecture, int LectureParts)
        {
            this.CourseId = CourseId;
            this.CourseName = CourseName;
            this.LecturePoints = LecturePoints;
            this.TAPoints = TAPoints;
            this.LectureOccurrences = LectureOccurrences;
            this.TAOccurrences = TAOccurrences;
            this.TAsAfterLecture = TAsAfterLecture;
            this.LectureParts = LectureParts;
        }
        public void InitCourse()
        {
            studentNumber = 0;
            studentCounter = 0;
            CourseStaff = new List<UniStaff>();
            tLo = TotalLectureOccurrences();

            foreach (string semester in Constants.Semesters)
            {
                if (LectureOccurrences[semester] > MaxPhase[semester])
                {
                    MaxPhase[semester] = LectureOccurrences[semester];
                }
            }

            tTAo = TotalTAOccurrences();

            NonAvailableStudants = new Dictionary<Period, List<int>>();
            InitNonAvailableStudants();
            /*
            progressTableLeactures = new Dictionary<string, Dictionary<int, int>>();
            progressTableTAs = new Dictionary<string, Dictionary<int, int>>();
            InitProgressTables();*/

            NonOverlappingCourses = new Dictionary<string, Dictionary<int, HashSet<Course>>>();
            TheUnoverlapableTimes = new Dictionary<string, Dictionary<int, List<Period>>>();
            InitOverlappingPhases();
        }

        private void InitOverlappingPhases()
        {
            foreach (string semester in Constants.Semesters)
            {
                NonOverlappingCourses[semester] = new Dictionary<int, HashSet<Course>>();
                TheUnoverlapableTimes[semester] = new Dictionary<int, List<Period>>();
                for (int phase = 1; phase <= LectureOccurrences[semester]; phase++)
                {
                    NonOverlappingCourses[semester][phase] = new HashSet<Course>() { this };
                    TheUnoverlapableTimes[semester][phase] = new List<Period>();
                }
            }
        }

        private void InitNonAvailableStudants()
        {
            foreach (Period period in Constants.AvailablePeriods)
            {
                var whatStudantsWant = new List<int>();
                for (int i = 0; i < tLo; i++)
                {
                    whatStudantsWant.Add(0);
                }
                NonAvailableStudants.Add(period, whatStudantsWant);
            }
        }
        /*
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
        }*/

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
            studentNumber++;
            if (student.UnavailableTimes == null)
            {
                return;
            }
            foreach (Period period in student.UnavailableTimes)
            {
                for (int hour = period.StartTime; hour < period.EndTime; hour++)
                {
                    NonAvailableStudants[
                        Constants.AvailablePeriodsFromPeriod[(period.Day, period.Semester, hour)]]
                        [studentCounter]++;
                }
            }
            studentCounter++;
            studentCounter %= tLo;
        }

        public void UpdateOverlappingCourses(Major major)
        {
            int year = major.InWhatYearIncluded(this);
            if (-1 == year)
            {
                return;
            }
            foreach (string semester in Constants.Semesters)
            {
                for (int phase = 1; phase <= LectureOccurrences[semester]; phase++)
                {
                    foreach (var bundle in major.Bundles[year])
                    {
                        NonOverlappingCourses[semester][phase].UnionWith(bundle.SampleCourses(phase));
                    }
                }
            }
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

        private int TotalTAOccurrences()
        {
            int sum = 0;
            foreach (var semester in Constants.Semesters)
            {
                sum += TAOccurrences[semester];
            }
            return sum;
        }

        public void UpdateCourseStaff(UniStaff employee)
        {
            CourseStaff.Add(employee);
        }

        public Period GetPracticPeriod(Period period, string role)
        {
            if (role == Constants.LecturerRole)
            {
                return new Period
                (
                    period.Day,
                    period.Semester,
                    period.StartTime,
                    period.StartTime + (LecturePoints / LectureParts)
                );
            }
            return new Period
            (
                period.Day,
                period.Semester,
                period.StartTime,
                period.StartTime + TAPoints
            );
        }

        public UniStaff? FindStaff(Period period, string role)
        {
            Period realPeriod = GetPracticPeriod(period, role);
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

        public List<UniStaff> GetAllByRole(string role)
        {
            List<UniStaff> list = new();
            foreach (var employee in CourseStaff)
            {
                if (employee.IsSomeRole(this, role))
                {
                    list.Add(employee);
                }
            }
            return list;
        }

        public UniStaff? GetByRole(string role)
        {
            foreach (UniStaff employee in CourseStaff)
            {
                if (employee.IsSomeRole(this, role))
                {
                    return employee;
                }
            }
            return null;
        }

        public bool IsOverlapping(Period period, string role, int phase)
        {
            Period realPeriod = GetPracticPeriod(period, role);

            if (period.IsTheDayEnds())
            {
                return true;
            }

            HashSet<Period> possibleOverlaps = new();
            foreach (var course in NonOverlappingCourses[period.Semester][phase])
            {
                foreach (var per in course.TheUnoverlapableTimes[period.Semester][phase])
                {
                    possibleOverlaps.Add(per);
                }
            }
            // can improve by saving possibleOverlaps
            return realPeriod.IsPeriodOverlap(possibleOverlaps);
        }

        public bool IsStudentsAvailable(Period period, int occurrencesIndex, string role)
        {
            double ratio = studentNumber / (double)tLo;
            int addition = LecturePoints;
            if (role == Constants.TARole)
            {
                ratio = studentNumber / (double)tTAo;
                addition = TAPoints;
            }
            List<Period> periods = new() { period };
            for (int i = period.StartTime + 1; i < period.StartTime + addition; i++)
            {
                periods.Add(new Period(period.Day, period.Semester, i, i + 1));
            }

            bool flag = true;
            foreach (Period per in periods)
            {
                if (NonAvailableStudants[per][occurrencesIndex - 1] > 0.3 * ratio)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                return true;
            }
            foreach (Period per in periods)
            {
                foreach (var studentProblems in NonAvailableStudants[per])
                {
                    if (studentProblems > 0.6 * ratio)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsStudentsAvailableForTheRemainingTas(Period period)
        {
            double ratio = studentNumber / (double) tTAo;
            int addition = TAPoints;
            List<Period> periods = new() { period };
            for (int i = period.StartTime + 1; i < period.StartTime + addition; i++)
            {
                periods.Add(new Period(period.Day, period.Semester, i, i + 1));
            }

            foreach (Period per in periods)
            {
                foreach (var studentProblems in NonAvailableStudants[per])
                {
                    if (studentProblems > 0.8 * ratio)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void UpdateUnoverlapableTimes(Period period, string role, int phase)
        {
            Period practicPeriod = GetPracticPeriod(period, role);
            TheUnoverlapableTimes[period.Semester][phase].Add(practicPeriod);
        }
        /*
        public void UpdateProgress(string semester, string role, int groupNum)
        {
            if (role == Constants.LecturerRole)
            {
                progressTableLeactures[semester][groupNum]--;
            }
            if (role == Constants.TARole)
            {
                progressTableTAs[semester][groupNum]--;
            }
        }*/
    }
}
