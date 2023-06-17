using CourseModel;

namespace ScheduleForStudent
{
    public class StudentDemands
    {
        public List<Period> UnavailableTimes { get; set; }
        public List<string> WantedCourses { get; set; }
        public int PreferedNumberOfLearningDays { get; set; }
        public int PreferedNumberOfHoursPerDay { get; set; }
        public List<string> WantedSemesters { get; set; }

        public StudentDemands(List<Period> unavailableTimes, List<string> wantedCourses,
            int preferedNumberOfLearningDays, int preferedNumberOfHoursPerDay,
            List<string> wantedSemesters)
        {
            UnavailableTimes = unavailableTimes;
            WantedCourses = wantedCourses;
            PreferedNumberOfLearningDays = preferedNumberOfLearningDays;
            PreferedNumberOfHoursPerDay = preferedNumberOfHoursPerDay;
            WantedSemesters = wantedSemesters;
        }
    }
}