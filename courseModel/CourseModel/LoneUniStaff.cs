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
        // map(course) -> map(role) -> course ocurrences
        public Dictionary<Course, Dictionary<string, int>> CoursesRolesOccurrences { get; set; }
        // map(course) -> map(role) -> course parts for the round
        private Dictionary<Course, int> CoursesLectureParts { get; set; }

        public LoneUniStaff(List<Period> unavailableTimes, Dictionary<Course,
            Dictionary<string, int>> coursesRolesOccurrences)
        {
            UnavailableTimes = unavailableTimes;
            CoursesRolesOccurrences = coursesRolesOccurrences;

            CoursesLectureParts = new Dictionary<Course, int>();
            foreach (Course course in CoursesRolesOccurrences.Keys)
            {
                if (CoursesRolesOccurrences[course].ContainsKey(Constants.LecturerRole))
                {
                    CoursesLectureParts[course] = course.LectureParts;
                }
            }
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
            if (role == Constants.LecturerRole)
            {
                CoursesLectureParts[course]--;
                if (0 == CoursesLectureParts[course])
                {
                    CoursesRolesOccurrences[course][role]--;
                    CoursesLectureParts[course] = course.LectureParts;
                }
            }
            if (role == Constants.TARole)
            {
                CoursesRolesOccurrences[course][role]--;
            }
            UnavailableTimes.Add(period);
        }
    }
}
