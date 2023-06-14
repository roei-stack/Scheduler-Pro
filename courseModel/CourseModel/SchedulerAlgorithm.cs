using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class SchedulerAlgorithm
    {
        public Dictionary<Course, CourseScheduling> SuperCourses { get; set; }
        public string AlgorithmMessage;
        public InstitutionInput Input { get; set; }

        public SchedulerAlgorithm(InstitutionInput input)
        {
            AlgorithmMessage = Constants.OverlapSuccess;
            Input = input;
            InputParser(input);
            SuperCourses = new Dictionary<Course, CourseScheduling>();
            foreach (var course in input.CourseList)
            {
                SuperCourses.Add(course, new CourseScheduling(course));
            }
        }

        public void Run()
        {
            // fisrt make sure that every course has group with no overlapping
            List<Course> needToScheduleTA = ScheduleOneForEveryCourse();
            ScheduleFirstTA(needToScheduleTA);
        }

        private void AssignSpecificTA(Course course, bool studentsImportant)
        {
            foreach (var period in Constants.AvailablePeriods)
            {
                if (course.IsOverlapping(period, Constants.TARole))
                {
                    continue;
                }
                if (studentsImportant && !course.IsStudentsAvailable(period, 1, Constants.TARole))
                {
                    continue;
                }
                UniStaff? ta = course.FindStaff(period, Constants.TARole);
                if (ta == null)
                {
                    continue;
                }
                // assign TA
                course.UpdateUnoverlapableTimes(period, Constants.TARole);
                Schedule(ta, course, period, 1, Constants.TARole);
                course.TAsAfterLecture--;
                course.FirstTAAssigned = true;
                break;
            }
        }

        private void ScheduleFirstTA(List<Course> needToScheduleTA)
        {
            foreach (var course in needToScheduleTA)
            {
                AssignSpecificTA(course, true);
                if (course.FirstTAAssigned == false)
                {
                    AssignSpecificTA(course, false);
                }
                if (course.FirstTAAssigned == false)
                {
                    AlgorithmMessage = Constants.OverlapFail;
                }
            }
        }

        private static void InputParser(InstitutionInput input)
        {
            foreach (var course in input.CourseList)
            {
                foreach (var student in input.Students)
                {
                    if (student.WantedCourses.Contains(course))
                    {
                        course.UpdateStudentProblems(student);
                    }
                }
                foreach (var major in input.Majors)
                {
                    course.UpdateOverlappingCourses(major);
                }
                foreach (var employee in input.Staff)
                {
                    if (employee.IsTeachingCourse(course))
                    {
                        course.UpdateCourseStaff(employee);
                    }
                }
            }
        }

        private List<Course> ScheduleOneForEveryCourse()
        {
            int lecParts;
            List<int> otherDays = new();
            List<Course> needToScheduleTA = new();
            foreach (var course in SuperCourses.Keys)
            {
                otherDays.Clear();
                lecParts = 0;
                lecParts = ScheduleOneForSpecificCourse(course, true, true, lecParts, otherDays);
                if (lecParts != course.LectureParts)
                {
                    lecParts = ScheduleOneForSpecificCourse(course, true, false, lecParts, otherDays);
                }
                if (lecParts != course.LectureParts)
                {
                    lecParts = ScheduleOneForSpecificCourse(course, false, true, lecParts, otherDays);
                }
                if (lecParts != course.LectureParts)
                {
                    lecParts = ScheduleOneForSpecificCourse(course, false, false, lecParts, otherDays);
                }
                if (lecParts != course.LectureParts)
                {
                    AlgorithmMessage = Constants.OverlapFail;
                }
                if (!course.FirstTAAssigned)
                {
                    needToScheduleTA.Add(course);
                }
            }
            return needToScheduleTA;
        }

        private int ScheduleOneForSpecificCourse(Course course,
            bool studentsImportant, bool TAAfterLecture, int lecPart, List<int> otherDays)
        {
            int hourForTA;
            for (int i = 0; lecPart != course.LectureParts && i < 2; i++)
            {
                foreach (Period period in Constants.AvailablePeriods)
                {
                    if (otherDays.Contains(period.Day))
                    {
                        continue;
                    }
                    if (lecPart == course.LectureParts)
                    {
                        break;
                    }
                    if (studentsImportant && !course.IsStudentsAvailable(period, 1, Constants.LecturerRole))
                    {
                        continue;
                    }
                    if (course.IsOverlapping(period, Constants.LecturerRole))
                    {
                        continue;
                    }
                    UniStaff? lecturer = course.FindStaff(period, Constants.LecturerRole);
                    if (null == lecturer)
                    {
                        continue;
                    }
                    if (!course.FirstTAAssigned && TAAfterLecture && course.TAsAfterLecture > 0)
                    {
                        hourForTA = period.StartTime + course.LecturePoints / course.LectureParts;
                        Period taPeriod = new(period.Day, period.Semester, hourForTA, hourForTA + 1);
                        if (course.IsOverlapping(taPeriod, Constants.TARole))
                        {
                            continue;
                        }
                        if (studentsImportant && !course.IsStudentsAvailable(taPeriod, 1, Constants.TARole))
                        {
                            continue;
                        }
                        UniStaff? ta = course.FindStaff(taPeriod, Constants.TARole);
                        if (ta == null)
                        {
                            continue;
                        }
                        // assign TA
                        course.UpdateUnoverlapableTimes(taPeriod, Constants.TARole);
                        Schedule(ta, course, taPeriod, 1, Constants.TARole);
                        course.TAsAfterLecture--;
                        course.FirstTAAssigned = true;
                    }
                    // assign for lecture
                    course.UpdateUnoverlapableTimes(period, Constants.LecturerRole);
                    Schedule(lecturer, course, period, 1, Constants.LecturerRole);
                    lecPart++;
                    otherDays.Add(period.Day);
                }
            }
            return lecPart;
        }

        private void Schedule(UniStaff staff, Course course, Period period, int groupNum, string role)
        {
            Period finalPeriod = course.GetPracticPeriod(period, role);
            staff.Schedule(course, Constants.LecturerRole, finalPeriod);

            course.UpdateProgress(period.Semester, role, groupNum);

            // update super class
            if (role == Constants.TARole)
            {
                groupNum += course.tLo;
            }
            if (SuperCourses[course].CourseGroups.ContainsKey(groupNum))
            {
                SuperCourses[course].CourseGroups[groupNum].Item2.Add(period);
            }
            else
            {
                SuperCourses[course].CourseGroups.Add(groupNum, (staff, new List<Period>(), role));
            }
        }
    }
}
