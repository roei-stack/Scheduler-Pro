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
        private Dictionary<Course, bool> gotBasicScheduling;
        public InstitutionInput Input { get; set; }

        public SchedulerAlgorithm(InstitutionInput input)
        {
            Input = input;
            InputParser(input);
            SuperCourses = new Dictionary<Course, CourseScheduling>();
            gotBasicScheduling = new Dictionary<Course, bool>();
            foreach (var course in input.CourseList)
            {
                SuperCourses.Add(course, new CourseScheduling(course));
                gotBasicScheduling.Add(course, false);
            }
        }

        public void Run()
        {
            // fisrt make sure that every course has group with no overlapping
            ScheduleOneForEveryCourse();
        }

        private void InputParser(InstitutionInput input)
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
                course.CleanThisFromOverlappingCourses();
                foreach (var employee in input.Staff)
                {
                    if (employee.IsTeachingCourse(course))
                    {
                        course.UpdateCourseStaff(employee);
                    }
                }
            }
        }

        private void ScheduleOneForEveryCourse()
        {
            int lecPart;
            int hourForTA;
            bool taAlso = false;
            foreach (var course in SuperCourses.Keys)
            {
                if (course.TAsAfterLecture > 0)
                {
                    taAlso = true;
                }
                lecPart = 0;

                for (int i = 0; lecPart != course.LectureParts && i < 2; i++)
                {
                    foreach (Period period in Constants.AvailablePeriods)
                    {
                        if (lecPart == course.LectureParts)
                        {
                            break;
                        }
                        if (!course.IsStudentsAvailable(period, 1))
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
                        if (taAlso)
                        {
                            hourForTA = period.StartTime + course.LecturePoints / course.LectureParts;
                            Period taPeriod = new(period.Day, period.Semester, hourForTA, hourForTA + 1);
                            if (course.IsOverlapping(taPeriod, Constants.TARole))
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
                            taAlso = false;
                        }
                        // assign for lecture
                        course.UpdateUnoverlapableTimes(period, Constants.LecturerRole);
                        Schedule(lecturer, course, period, 1, Constants.LecturerRole);
                        gotBasicScheduling[course] = true;
                        lecPart++;
                    }
                }
            }
        }

        private void Schedule(UniStaff staff, Course course, Period period, int groupNum, string role)
        {
            Period finalPeriod = course.GetPracticPeriod(period, role);
            staff.Schedule(course, Constants.LecturerRole, finalPeriod);

            course.UpdateProgress(period.Semester, role, groupNum);

            // update super class
            if (role == Constants.TARole)
            {
                groupNum += course.tlo;
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
