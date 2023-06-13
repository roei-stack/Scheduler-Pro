using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class LoneUniStaff : UniStaff
    {
        public string? Name { get; set; }
        public string? ID { get; set; }
        // staff's input
        // map(semester) -> times
        public List<Period> UnavailableTimes { get; set; }
        // institution's input
        // map(course) -> map(role) -> course ccurrences
        public Dictionary<Course, Dictionary<string, int>> CoursesRolesOccurrences { get; set; }

        public LoneUniStaff(List<Period> unavailableTimes, Dictionary<Course,
            Dictionary<string, int>> coursesRolesOccurrences)
        {
            UnavailableTimes = unavailableTimes;
            CoursesRolesOccurrences = coursesRolesOccurrences;
        }


        public bool IsPeriodAvailable(Period period)
        {
            foreach (var unavailablePeriod in UnavailableTimes)
            {
                // Check if the periods overlap
                if (period.Day == unavailablePeriod.Day &&
                    period.Semester == unavailablePeriod.Semester &&
                    period.StartTime < unavailablePeriod.EndTime &&
                    period.EndTime > unavailablePeriod.StartTime)
                {
                    // Overlapping periods found
                    return false;
                }
            }
            // No overlapping periods found
            return true;
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
