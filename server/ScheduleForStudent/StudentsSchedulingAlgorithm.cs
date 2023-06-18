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
        private Dictionary<string, CourseProperties> Courses;
        private Dictionary<Course, CourseScheduling> SuperCourses;
        public StudentScheduling ProStudentScheduling { get; set; }
        private List<Period> unoverlapablePeriods;
        /// <summary>
        /// id, semester
        /// </summary>
        private Dictionary<string, string> semesterTable;
        private Dictionary<string, int> progressTable;
        private bool fromInstutution;
        private HashSet<string> currentCoursesIds;

        public StudentsSchedulingAlgorithm(StudentDemands demands,
                 Dictionary<Course, CourseScheduling> superCourses)
        {
            Demands = demands;
            SuperCourses = superCourses;
            ProStudentScheduling = new StudentScheduling();
            unoverlapablePeriods = new List<Period>();
            progressTable = new Dictionary<string, int>();
            semesterTable = new Dictionary<string, string>();
            foreach (Course course in superCourses.Keys)
            {
                progressTable[course.CourseId] = 0;
            }
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
            CompleteRemainingCourses(ProStudentScheduling);
        }

        private void ScheduleFromInstutution()
        {
            HashSet<string> bestCurrentCoursesIds = new();
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
                    currentCoursesIds.Add(course.CourseId);
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
            unoverlapablePeriods.AddRange(SuperCourses[course]
                .CourseGroups[groupNumber].Item2);

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
            ScheduleClasses(true, helpScheduling);
            ScheduleClasses(false, helpScheduling);
            FindRemaningExercises(true, helpScheduling);
            FindRemaningExercises(false, helpScheduling);
        }

        private void ScheduleClasses(bool studentImportant, StudentScheduling SS)
        {
            foreach (string semester in Demands.WantedSemesters)
            {
                foreach (Period period in Constants.AvailablePeriods.Where(
                    period => (period.Semester == semester)))
                {
                    FindLectures(period, studentImportant, SS);
                }
            }
        }

        private void FindRemaningExercises(bool studentImportant, StudentScheduling SS)
        {
            int groupNumber;
            foreach (string semester in Demands.WantedSemesters)
            {
                foreach (Period period in Constants.AvailablePeriods.Where(
                    period => (period.Semester == semester)))
                {
                    foreach (string courseID in Demands.WantedCourses.Where(id =>
                            (progressTable[id] == 1 && semesterTable[id] == semester
                            && Courses[id].Duration[Constants.Exercise] != 0)))
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
                        Schedule(courseID, groupNumber, Constants.Exercise, SS);
                    }
                }
            }
        }

        private bool FindLectures(Period period, bool studentImportant, StudentScheduling SS)
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
                             (progressTable[id] == 0) && !currentCoursesIds.Contains(id)))
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
                Schedule(courseID, groupNumber, Constants.Lecture, SS);

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
                Schedule(courseID, taGroupNumber, Constants.Exercise, SS);
                return true;
            }
            return false;
        }

        private void Schedule(string courseId, int groupNumber, string classType,
            StudentScheduling SS)
        {
            unoverlapablePeriods.AddRange(Courses[courseId].
                Groups[classType][groupNumber]);

            currentCoursesIds.Add(courseId);

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
                    SS.Schedule[per] = input;
                }
            }

        }
    }
}
