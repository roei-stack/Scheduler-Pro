namespace server.Models
{
    public class LoneUniStaff : UniStaff
    {
        public string? Name { get; set; }
        public string? ID { get; set; }
        // staff's input
        // map(semester) -> times
        public List<Period> UnavailableTimes { get; set; }
        // institution's input
        // map(course) -> map(role) -> course ocurrences
        public Dictionary<Course, Dictionary<string, int>> CoursesRolesOccurrences { get; set; }

        public LoneUniStaff(List<Period> unavailableTimes, Dictionary<Course,
            Dictionary<string, int>> coursesRolesOccurrences)
        {
            UnavailableTimes = unavailableTimes;
            CoursesRolesOccurrences = coursesRolesOccurrences;
        }

        public bool IsPeriodAvailable(Period period)
        {
            return !period.IsPeriodOverlap(UnavailableTimes);
        }

        public bool IsTeachingCourse(Course course)
        {
            return CoursesRolesOccurrences.ContainsKey(course);
        }

        public bool IsSomeRole(Course course, string role)
        {
            if (CoursesRolesOccurrences[course].ContainsKey(role))
            {
                if (CoursesRolesOccurrences[course][role] > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void Schedule(Course course, string role, Period period)
        {
            CoursesRolesOccurrences[course][role]--;
            UnavailableTimes.Add(period);
        }
    }
}
