using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class CourseBundle
    {
        public string ID { get; set; }
        public int MinCreditPoints { get; set; }
        public int MaxCreditPoints { get; set; }
        public List<Course> Courses { get; set; }
        private int totalCreditPoints = 0;
        private bool isNotInited = true;

        public CourseBundle(string id, List<Course> courses)
        {
            ID = id;
            Courses = courses;
        }
        public CourseBundle(string id, int minCreditPoints, int maxCreditPoints, List<Course> courses)
        {
            ID = id;
            MinCreditPoints = minCreditPoints;
            MaxCreditPoints = maxCreditPoints;
            Courses = courses;
        }

        public void InitBundle()
        {
            if (isNotInited)
            {
                foreach (Course course in Courses)
                {
                    totalCreditPoints += course.CreditPoints();
                }
                isNotInited = false;
            }
        }

        public HashSet<Course> SampleCourses(int phase)
        {
            int len = Courses.Count;
            int[] indexArray = Enumerable.Range(0, len).ToArray();
            MixArray(indexArray);
            int totalCreditPoints = 0;
            int limit = Math.Min(MinCreditPoints + 4 - phase,
                Math.Min(MaxCreditPoints, totalCreditPoints));
            HashSet<Course> result = new();
            for (int i = 0; totalCreditPoints < limit; i++)
            {
                result.Add(Courses[i]);
                totalCreditPoints += Courses[i].CreditPoints();
            }
            return result;
        }

        public static void MixArray(int[] array)
        {
            Random random = new();

            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);

                // Swap elements at index i and j
                (array[j], array[i]) = (array[i], array[j]);
            }
        }
    }
}
