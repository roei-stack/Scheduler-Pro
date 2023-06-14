namespace server.Models
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
        // for using to what lecture group
        private int studentCounter;
        public int tlo;
        // map(semster) -> map(course group) -> part that left
        private Dictionary<string, Dictionary<int, int>> progressTableLeactures;
        private Dictionary<string, Dictionary<int, int>> progressTableTAs;
        public List<Period> TheUnoverlapableTimes;
        // how many students are in this course
        private int studentNumber;

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

            studentNumber = 0;
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

            TheUnoverlapableTimes = new List<Period>();
        }

        private void InitNonAvailableStudants()
        {
            foreach (Period period in Constants.AvailablePeriods)
            {
                var whatStudantsWant = new List<int>();
                for (int i = 0; i < tlo; i++)
                {
                    whatStudantsWant.Add(0);
                }
                NonAvailableStudants.Add(period, whatStudantsWant);
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
            studentNumber++;
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

        public bool IsOverlapping(Period period, string role)
        {
            Period realPeriod = GetPracticPeriod(period, role);

            if (period.IsTheDayEnds())
            {
                return true;
            }

            HashSet<Period> possibleOverlaps = new();
            foreach (var course in NonOverlappingCourses)
            {
                foreach (var per in course.TheUnoverlapableTimes)
                {
                    possibleOverlaps.Add(per);
                }
            }
            return realPeriod.IsPeriodOverlap(possibleOverlaps);
        }

        public bool IsStudentsAvailable(Period period, int occurrencesIndex)
        {
            double ratio = studentNumber / (double)tlo;
            if (NonAvailableStudants[period][occurrencesIndex - 1] > 0.8 * ratio)
            {
                return true;
            }
            foreach (var studentProblems in NonAvailableStudants[period])
            {
                if (studentProblems < 0.5 * ratio)
                {
                    return false;
                }
            }
            return true;
        }

        public void UpdateUnoverlapableTimes(Period period, string role)
        {
            Period practicPeriod = GetPracticPeriod(period, role);
            TheUnoverlapableTimes.Add(practicPeriod);
        }

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
        }
    }
}
