using CourseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleForStudent
{
    public class StudentsSchedulingAlgorithm
    {
        public StudentDemands Demands { get; set; }
        private Dictionary<string, CourseProperties>? Courses;
        private Dictionary<Course, CourseScheduling>? SuperCourses;
        public StudentScheduling ProStudentScheduling { get; set; }
        private List<Period> unoverlapablePeriods;
        /// <summary>
        /// id, semester
        /// </summary>
        private Dictionary<string, string> semesterTable;
        private Dictionary<string, int> progressTable;
        private bool fromInstutution;

        public StudentsSchedulingAlgorithm(StudentDemands demands,
                 Dictionary<Course, CourseScheduling>? superCourses)
        {
            Demands = demands;
            SuperCourses = superCourses;
            ProStudentScheduling = new StudentScheduling();
            unoverlapablePeriods = new List<Period>();
            progressTable = new Dictionary<string, int>();
            semesterTable = new Dictionary<string, string>();
            fromInstutution = true;
        }

        public StudentsSchedulingAlgorithm(StudentDemands demands,
                    Dictionary<string, CourseProperties> courses)
        {
            fromInstutution = false;
            Demands = demands;
            Courses = courses;
            ProStudentScheduling = new StudentScheduling();
            unoverlapablePeriods = new List<Period>();
            progressTable = new Dictionary<string, int>();
            semesterTable = new Dictionary<string, string>();
            foreach (string courseId in Courses.Keys)
            {
                progressTable[courseId] = 0;
            }
        }

        public void Run()
        {
            if (fromInstutution)
            {
                ScheduleFromInstutution();
                return;
            }
            ScheduleClasses(true);
            ScheduleClasses(false);
            FindRemaningExercises(true);
            FindRemaningExercises(false);
        }

        private void ScheduleFromInstutution()
        {
            List<string> currentCoursesIds;
            List<string> bestCurrentCoursesIds = new();
            int groupNumber = 1;
            bool existGroupNumber = true;
            StudentScheduling helpScheduling;
            while (existGroupNumber)
            {
                helpScheduling = new StudentScheduling();
                currentCoursesIds = new();
                foreach (Course course in SuperCourses.Keys.Where(course =>
                                         Demands.WantedCourses.Contains(course.CourseId))) {
                    if (!SuperCourses[course].CourseGroups.ContainsKey(groupNumber + course.tLo))
                    {
                        existGroupNumber = false;
                        continue;
                    }
                    existGroupNumber = true;
                    ScheduleCourseGroupFromInstitution(course, groupNumber, helpScheduling);
                    ScheduleCourseGroupFromInstitution(course, groupNumber + course.tLo, helpScheduling);
                }
                CompleteRemainingCourses(helpScheduling);
                groupNumber++;
                if (ProStudentScheduling.Evaluate(Demands, currentCoursesIds)
                    < helpScheduling.Evaluate(Demands, bestCurrentCoursesIds))
                {
                    ProStudentScheduling = helpScheduling;
                    bestCurrentCoursesIds = currentCoursesIds;
                }
            }
        }

        private void ScheduleCourseGroupFromInstitution(Course course,
                    int groupNumber, StudentScheduling helpScheduling)
        {
            string classType = Constants.Lecture;
            Period per;
            if (SuperCourses[course].CourseGroups[groupNumber].Item3 == Constants.TARole)
            {
                classType = Constants.Exercise;
            }
            if (SuperCourses[course].CourseGroups[groupNumber].Item3 == Constants.LecturerRole)
            {
                classType = Constants.Lecture;
            }
            (string, int, CourseProperties) input = (classType, groupNumber,
                            new CourseProperties(course));
            foreach (Period period in SuperCourses[course]
                .CourseGroups[groupNumber].Item2)
            {
                for (int hour = period.StartTime; hour < period.EndTime; hour++)
                {
                    per = Constants.AvailablePeriodsFromPeriod
                                    [(period.Day, period.Semester, hour)];
                    helpScheduling.Schedule[per] = input;
                }
            }
        }

        private void CompleteRemainingCourses(StudentScheduling helpScheduling)
        {

        }

        private void ScheduleClasses(bool studentImportant)
        {
            foreach (string semester in Demands.WantedSemesters)
            {
                foreach (Period period in Constants.AvailablePeriods.Where(
                    period => (period.Semester == semester)))
                {
                    FindLectures(period, studentImportant);
                }
            }
        }

        private void FindRemaningExercises(bool studentImportant)
        {
            int groupNumber;
            foreach (string semester in Demands.WantedSemesters)
            {
                foreach (Period period in Constants.AvailablePeriods.Where(
                    period => (period.Semester == semester)))
                {
                    foreach (string courseID in Demands.WantedCourses.Where(id =>
                            (progressTable[id] == 1 && semesterTable[id] == semester)))
                    {
                        groupNumber = Courses[courseID].
                            FindGroupByType(Constants.Exercise, period);
                        if (groupNumber == -1)
                        {
                            continue;
                        }
                        if (Period.AreOverlaping(unoverlapablePeriods, Courses[courseID].
                                Groups[Constants.Exercise][groupNumber]))
                        {
                            continue;
                        }
                        Schedule(courseID, groupNumber, Constants.Exercise);
                    }
                }
            }
        }

        private bool FindLectures(Period period, bool studentImportant)
        {
            if (studentImportant && period.IsPeriodOverlap(Demands.UnavailableTimes))
            {
                return false;
            }
            int groupNumber;
            int taGroupNumber;
            Period taPeriod;
            // try to get lecture then exercise first
            foreach (string courseID in Demands.WantedCourses.Where(id =>
                                    (progressTable[id] == 0)))
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
                Schedule(courseID, groupNumber, Constants.Lecture);

                // find ta
                taPeriod = Constants.AvailablePeriodsFromPeriod[(period.Day, period.Semester,
                    period.StartTime + Courses[courseID].Duration[Constants.Lecture])];
                taGroupNumber = Courses[courseID].FindGroupByType(Constants.Exercise, taPeriod);
                if (taGroupNumber == -1)
                {
                    return true;
                }
                if (Period.AreOverlaping(unoverlapablePeriods, Courses[courseID].
                                        Groups[Constants.Exercise][taGroupNumber]))
                {
                    return true;
                }
                Schedule(courseID, taGroupNumber, Constants.Exercise);
                return true;
            }
            return false;
        }

        private void Schedule(string courseId, int groupNumber, string classType)
        {
            unoverlapablePeriods.AddRange(Courses[courseId].
                Groups[classType][groupNumber]);

            progressTable[courseId]++;
            semesterTable[courseId] = Courses[courseId].
                                Groups[classType][groupNumber][0].Semester;
            Period per;
            (string, int, CourseProperties) input = (classType, groupNumber, Courses[courseId]);
            foreach (Period period in Courses[courseId].Groups[classType][groupNumber])
            {
                for (int hour = period.StartTime;
                    hour < period.StartTime + Courses[courseId].Duration[classType]; hour++)
                {
                    per = Constants.AvailablePeriodsFromPeriod
                                    [(period.Day, period.Semester, hour)];
                    ProStudentScheduling.Schedule[per] = input;
                }
            }

        }
    }
}
