using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class MultipleUniStaff : UniStaff
    {
        public List<UniStaff> UniStaffList { get; set; }

        public MultipleUniStaff() { UniStaffList = new List<UniStaff>(); }

        public bool IsPeriodAvailable(Period period)
        {
            foreach (var staff in UniStaffList)
            {
                if(!staff.IsPeriodAvailable(period))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
