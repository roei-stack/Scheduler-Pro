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
        /*
        public void RunV1()
        {
            // fisrt make sure that every course has group with no overlapping
            List<Course> needToScheduleTA = ScheduleOneForEveryCourse();
            ScheduleFirstTA(needToScheduleTA);
        }
        */
        public void Run()
        {
            ScheduleLectures();
            ScheduleRemainingTAs();
        }

        private void ScheduleRemainingTAs()
        {
            UniStaff? ta;
            foreach (string semester in Constants.Semesters)
            {
                foreach (Course course in SuperCourses.Keys.Where(
                    course => (course.TAOccurrences[semester] > 0)))
                {
                    while (course.TAOccurrences[semester] > 0)
                    {
                        foreach (Period period in Constants.AvailablePeriods.Where(
                                  period => (period.Semester == semester)))
                        {
                            ta = course.FindStaff(period, Constants.TARole);
                            if (ta == null)
                            {
                                continue;
                            }
                            if (!course.IsStudentsAvailableForTheRemainingTas(period))
                            {
                                continue;
                            }
                            Period realPeriod = course.GetPracticPeriod(period, Constants.TARole);
                            if (realPeriod.IsTheDayEnds())
                            {
                                continue;
                            }
                            Schedule(ta, course, realPeriod, course.currentGroupNumber, Constants.TARole);
                            course.currentGroupNumber++;
                            course.TAOccurrences[semester]--;
                        }
                    }
                }
            }
        }

        private void ScheduleLectures()
        {
            int currentPart;
            List<int> otherDays = new();
            int tryOutput;
            foreach (string semester in Constants.Semesters)
            {
                for (int phase = 1; phase < Course.MaxPhase[semester]; phase++)
                {
                    foreach (Course course in SuperCourses.Keys.
                            Where(course => (course.LectureOccurrences[semester] > 0)))
                    {
                        otherDays.Clear();
                        if (course.LectureOccurrences[semester] > 0)
                        {
                            continue;
                        }
                        if (course.TAsAfterLecture > 0)
                        {
                            currentPart = 0;
                            tryOutput = TryScheduleLectureThenTA(course, phase, semester, true,
                                    otherDays, ref currentPart);
                            if (tryOutput == 2)
                            {
                                course.LectureOccurrences[semester]--;
                                course.TAOccurrences[semester]--;
                                course.currentGroupNumber += 2;
                                continue;
                            }
                            if (tryOutput == 1)
                            {
                                TryScheduleLectureThenTA(course, phase, semester, false,
                                    otherDays, ref currentPart);
                                course.LectureOccurrences[semester]--;
                                course.TAOccurrences[semester]--;
                                course.currentGroupNumber += 2;
                                continue;
                            }
                        }
                        foreach (string role in Constants.Roles)
                        {
                            UniStaff? employee = course.GetByRole(role);
                            if (employee != null)
                            {
                                currentPart = 0;
                                if (!ShcheduleStaff(employee, role, true, otherDays,
                                    course, phase, semester, ref currentPart))
                                {
                                    ShcheduleStaff(employee, role, false, otherDays,
                                        course, phase, semester, ref currentPart);
                                }
                                course.currentGroupNumber++;
                                if (role == Constants.LecturerRole)
                                {
                                    course.LectureOccurrences[semester]--;
                                }
                                if (role == Constants.TARole)
                                {
                                    course.TAOccurrences[semester]--;
                                }
                                
                            }
                            otherDays.Clear();
                        }
                    }
                }
            }
        }
        /* output
         * 0: faild to find TA
         * 1: found TA but faild to found the other lecture parts
         * 2: succsess
         */
        private int TryScheduleLectureThenTA(Course course, int phase, string semester,
            bool studentsImportant, List<int> otherDays, ref int currentPart)
        {
            // initilaize
            Period realPeriod;
            int hourForTA;
            UniStaff? lecturer = course.GetByRole(Constants.LecturerRole);
            if (lecturer == null)
            {
                return 0;
            }
            for (int i = 0; i < 2; i++)
            {
                if (currentPart == 0 && i == 1)
                {
                    break;
                }
                foreach (Period period in Constants.AvailablePeriods.Where(
                                  period => (period.Semester == semester)))
                {
                    if (currentPart == course.LectureParts)
                    {
                        return 2;
                    }
                    if (otherDays.Contains(period.Day))
                    {
                        continue;
                    }
                    realPeriod = course.GetPracticPeriod(period, Constants.LecturerRole);
                    if (!lecturer.IsPeriodAvailable(realPeriod))
                    {
                        continue;
                    }
                    if (course.IsOverlapping(period, Constants.LecturerRole, phase))
                    {
                        continue;
                    }
                    if (studentsImportant && !course.IsStudentsAvailable(period, phase,
                                                        Constants.LecturerRole))
                    {
                        continue;
                    }
                    if (currentPart == 0)
                    {
                        hourForTA = period.StartTime + course.LecturePoints / course.LectureParts;
                        Period taPeriod = new(period.Day, period.Semester, hourForTA, hourForTA + 1);
                        if (course.IsOverlapping(taPeriod, Constants.TARole, phase))
                        {
                            continue;
                        }
                        if (studentsImportant && !course.IsStudentsAvailable(taPeriod,
                                                    phase, Constants.TARole))
                        {
                            continue;
                        }
                        UniStaff? ta = course.FindStaff(taPeriod, Constants.TARole);
                        if (ta == null)
                        {
                            continue;
                        }
                        // assign TA
                        taPeriod = course.GetPracticPeriod(period, Constants.TARole);
                        course.UpdateUnoverlapableTimes(taPeriod, Constants.TARole, phase);
                        Schedule(ta, course, taPeriod, phase, Constants.TARole);
                        course.TAsAfterLecture--;
                    }
                    // assign
                    otherDays.Add(period.Day);
                    currentPart++;
                    course.UpdateUnoverlapableTimes(realPeriod, Constants.LecturerRole, phase);
                    Schedule(lecturer, course, realPeriod, phase, Constants.LecturerRole);
                }
            }
            return 0;
        }

        private bool ShcheduleStaff(UniStaff employee, string role, bool studentsImportant,
            List<int> otherDays, Course course, int phase, string semester, ref int currentPart)
        {
            // initilaize
            int parts = 1;
            if (role == Constants.LecturerRole)
            {
                parts = course.LectureParts;
            }
            Period realPeriod;

            // algo
            foreach (Period period in Constants.AvailablePeriods.Where(
                                              period => (period.Semester == semester)))
            {
                if (currentPart == parts)
                {
                    return true;
                }
                if (otherDays.Contains(period.Day))
                {
                    continue;
                }
                realPeriod = course.GetPracticPeriod(period, role);
                if (!employee.IsPeriodAvailable(realPeriod))
                {
                    continue;
                }
                if (course.IsOverlapping(period, role, phase))
                {
                    continue;
                }
                if (studentsImportant && !course.IsStudentsAvailable(period, phase, role))
                {
                    continue;
                }
                // assign
                otherDays.Add(period.Day);
                currentPart++;
                course.UpdateUnoverlapableTimes(realPeriod, role, phase);
                Schedule(employee, course, realPeriod, phase, role);
            }
            if (!studentsImportant)
            {
                AlgorithmMessage = Constants.OverlapFail;
            }
            return false;
        }
        /*
        private void AssignSpecificTA(Course course, bool studentsImportant)
        {
            foreach (var period in Constants.AvailablePeriods)
            {
                if (course.IsOverlapping(period, Constants.TARole, 1))
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
                course.UpdateUnoverlapableTimes(period, Constants.TARole, 1);
                Schedule(ta, course, period, 1, Constants.TARole);
                course.TAsAfterLecture--;
                course.FirstTAAssigned = true;
                break;
            }
        }
        */
        /*
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
        */
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
        /*
        private List<Course> ScheduleOneForEveryCourse()
        {
            int lecParts;
            List<int> otherDays = new();
            List<Course> needToScheduleTA = new();
            foreach (var course in SuperCourses.Keys)
            {
                otherDays.Clear();
                lecParts = 0;
                //UniStaff lecturer
                ScheduleOneForSpecificCourse(course, true, true, ref lecParts, otherDays);
                if (lecParts != course.LectureParts)
                {
                    ScheduleOneForSpecificCourse(course, true, false, ref lecParts, otherDays);
                }
                if (lecParts != course.LectureParts)
                {
                    ScheduleOneForSpecificCourse(course, false, true, ref lecParts, otherDays);
                }
                if (lecParts != course.LectureParts)
                {
                    ScheduleOneForSpecificCourse(course, false, false, ref lecParts, otherDays);
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

        private void ScheduleOneForSpecificCourse(Course course,
            bool studentsImportant, bool TAAfterLecture, ref int lecPart, List<int> otherDays)
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
                    if (course.IsOverlapping(period, Constants.LecturerRole, 1))
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
                        if (course.IsOverlapping(taPeriod, Constants.TARole, 1))
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
                        course.UpdateUnoverlapableTimes(taPeriod, Constants.TARole, 1);
                        Schedule(ta, course, taPeriod, 1, Constants.TARole);
                        course.TAsAfterLecture--;
                        course.FirstTAAssigned = true;
                    }
                    // assign for lecture
                    course.UpdateUnoverlapableTimes(period, Constants.LecturerRole, 1);
                    Schedule(lecturer, course, period, 1, Constants.LecturerRole);
                    lecPart++;
                    otherDays.Add(period.Day);
                }
            }
        }
        */
        private void Schedule(UniStaff staff, Course course, Period period, int groupNum, string role)
        {
            staff.Schedule(course, role, period);

            // course.UpdateProgress(period.Semester, role, groupNum);

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
