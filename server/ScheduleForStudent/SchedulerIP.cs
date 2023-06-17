using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseModel;
using Google.OrTools.LinearSolver;
/*
namespace ScheduleForStudent
{
    using System;
    using System.Collections.Generic;
    using CourseModel;
    using Google.OrTools.LinearSolver;

    using System;
    using System.Collections.Generic;
    using CourseModel;
    using Google.OrTools.LinearSolver;

    public class SchedulerIP
    {
        public StudentScheduling ScheduleCourses(List<CourseProperties> courses, StudentDemands studentDemands)
        {
            var solver = Solver.CreateSolver("SCIP");

            // Create decision variables
            var variables = new Dictionary<string, Dictionary<int, List<Variable>>>();

            foreach (var course in courses)
            {
                variables[course.ID] = new Dictionary<int, List<Variable>>();

                foreach (var groupEntry in course.Groups[Constants.Lecture])
                {
                    var groupNumber = groupEntry.Key;
                    var periods = groupEntry.Value;

                    variables[course.ID][groupNumber] = new List<Variable>();

                    foreach (var period in periods)
                    {
                        var variable = solver.MakeBoolVar($"{course.ID}_lecture_group{groupNumber}_period{period.Day}_{period.StartTime}_{period.EndTime}_{period.Semester}");
                        variables[course.ID][groupNumber].Add(variable);
                    }
                }

                foreach (var groupEntry in course.Groups[Constants.Exercise])
                {
                    var groupNumber = groupEntry.Key;
                    var periods = groupEntry.Value;

                    variables[course.ID][groupNumber] = new List<Variable>();

                    foreach (var period in periods)
                    {
                        var variable = solver.MakeBoolVar($"{course.ID}_exercise_group{groupNumber}_period{period.Day}_{period.StartTime}_{period.EndTime}_{period.Semester}");
                        variables[course.ID][groupNumber].Add(variable);
                    }
                }
            }

            // Define constraints
            foreach (var studentDemand in studentDemands.UnavailableTimes)
            {
                foreach (var courseVariables in variables.Values)
                {
                    foreach (var groupVariables in courseVariables.Values)
                    {
                        foreach (var variable in groupVariables)
                        {
                            var period = GetPeriodFromVariable(variable);

                            if (studentDemand.IsPeriodOverlap(new List<Period> { period }))
                            {
                                solver.Add(variable == 0);
                            }
                        }
                    }
                }
            }

            // Rest of the code...

            // Solve the ILP problem
            var resultStatus = solver.Solve();

            // Rest of the code...
        }

        private Period GetPeriodFromVariable(Variable variable)
        {
            var variableName = variable.Name();
            var parts = variableName.Split('_');

            var day = int.Parse(parts[4]);
            var startTime = int.Parse(parts[5]);
            var endTime = int.Parse(parts[6]);
            var semester = parts[7];

            // Rest of the code...

            return new Period(day, semester, startTime, endTime);
        }
    }

    // Rest of the code...
}*/
/*
// Example usage:
var schedulerIP = new SchedulerIP();
var scheduleIP = schedulerIP.ScheduleCourses(courses, studentDemands);
*/