using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseModel
{
    public class CourseBundle
    {
        public int MinCreditPoints { get; set; }
        public int MaxCreditPoints { get; set; }
        public List<Course> Courses { get; set; }
        public int Year { get; set; }
        public string? Semester { get; set; }

        public CourseBundle(int year, string semester)
        {
            Courses = new List<Course>();
            Year = year;
            Semester = semester;
        }

        public HashSet<Course> SampleCourses()
        {
            int len = Courses.Count;
            int[] indexArray = Enumerable.Range(0, len).ToArray();
            MixArray(indexArray);
            int totalCreditPoints = 0;
            int limit = Math.Min(MinCreditPoints + 2, MaxCreditPoints);
            HashSet<Course> result = new HashSet<Course>();
            for (int i = 0; totalCreditPoints <= limit; i++)
            {
                result.Add(Courses[i]);
                totalCreditPoints += Courses[i].CreditPoints();
            }
            return result;
        }

        public static void MixArray(int[] array)
        {
            Random random = new Random();

            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);

                // Swap elements at index i and j
                int temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
    }
}
