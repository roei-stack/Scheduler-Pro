using CourseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleForStudent
{
    public  class StudentsSchedulingAlgorithm
    {
        public StudentDemands Demands { get; set; }
        public Dictionary<string, CourseProperties> Courses { get; set; }
        public StudentScheduling StudentScheduling { get; set; }
        private List<Period> unoverlapablePeriods;

        public StudentsSchedulingAlgorithm(StudentDemands demands,
                    Dictionary<string, CourseProperties> courses)
        {
            Demands = demands;
            Courses = courses;
            StudentScheduling = new StudentScheduling();
            unoverlapablePeriods = new List<Period>();
        }

        public void Run()
        {
            
        }

        private void ScheduleClasses(bool studentImportance)
        {
            foreach (string semester in Demands.WantedSemesters)
            {
                foreach (Period period in Constants.AvailablePeriods.Where(
                    period => (period.Semester == semester)))
                {
                    for (int day = 1; day < Constants.MaxDay; day++)
                    {
                        
                    }
                }
            }
        }

        private (CourseProperties, int, int?)? FindCourse(Period period)
        {
            int groupNumber;
            int taGroupNumber = -1;
            Period taPeriod;
            // try to get lecture then exercise first
            foreach (string courseID in Courses.Keys)
            {
                groupNumber = Courses[courseID].FindGroupByType(Constants.Lecture, period);
                if (groupNumber == -1)
                {
                    continue;
                }
                if (Period.AreOverlaping(unoverlapablePeriods, Courses[courseID].
                    Groups[Constants.Lecture][groupNumber]))
                {
                    continue;
                }
                taPeriod = Constants.AvailablePeriodsFromPeriod[(period.Day, period.Semester,
                    period.StartTime + Courses[courseID].Duration[Constants.Lecture])];
                taGroupNumber = Courses[courseID].FindGroupByType(Constants.Exercise, taPeriod);
                if (taGroupNumber == -1)
                {
                    break;
                }
                if (Period.AreOverlaping(unoverlapablePeriods, Courses[courseID].
                                        Groups[Constants.Exercise][taGroupNumber]))
                {
                    break;
                }
                unoverlapablePeriods.AddRange(Courses[courseID].Groups[Constants.Exercise][taGroupNumber]);
            }

            return null;

            if (period.IsPeriodOverlap(Demands.UnavailableTimes))
            {
                return null;
            }
        }

    }
}
