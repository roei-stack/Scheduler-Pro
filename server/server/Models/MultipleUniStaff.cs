namespace server.Models
{
    public class MultipleUniStaff : UniStaff
    {
        public List<UniStaff> UniStaffList { get; set; }
        public List<Course> SharedCourses { get; set; }

        public MultipleUniStaff(List<Course> sharedCourses, List<UniStaff> staff)
        {
            UniStaffList = staff;
            SharedCourses = sharedCourses;
        }

        public bool IsPeriodAvailable(Period period)
        {
            foreach (var staff in UniStaffList)
            {
                if (!staff.IsPeriodAvailable(period))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsTeachingCourse(Course course)
        {
            return SharedCourses.Contains(course);
        }

        public bool IsSomeRole(Course course, string role)
        {
            if (role == Constants.LecturerRole)
            {
                foreach (var employee in UniStaffList)
                {
                    if (!employee.IsSomeRole(course, role))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public void Schedule(Course course, string role, Period period)
        {
            foreach (var employee in UniStaffList)
            {
                employee.Schedule(course, role, period);
            }
        }
    }
}
