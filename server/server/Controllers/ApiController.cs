using Microsoft.AspNetCore.Mvc;
using server.ViewModels;
using server.Models;
using server.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Text.Json;
using CourseModel;
using System.Collections.Generic;
using ScheduleForStudent;
/*
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Forms.v1;
using Google.Apis.Forms.v1.Data;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters;*/

namespace server.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly APIDbContext dbContext;
        private readonly KeyManager keyManager;

        public ApiController(APIDbContext dbContext, KeyManager keyManager)
        {
            this.dbContext = dbContext;
            this.keyManager = keyManager;
        }

        [HttpPost("Account/signup")]
        public async Task<IActionResult> SignUp([FromBody] User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                // currently
                model.NeedSetup = model.IsInstitution;
                await dbContext.Users.AddAsync(model);

                if (model.IsInstitution)
                {
                    var institutionData = new InstitutionData { Username = model.Username };
                    await dbContext.InstitutionsData.AddAsync(institutionData);
                }
                await dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("Account/signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await dbContext.Users.FindAsync(model.Username); // todo maybe only db.find()
            // todo hash passwords
            if (user != null && user.Password == model.Password)
            {
                var tokenExpiration = model.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1);
                string token = GenerateJWTToken(user, tokenExpiration);
                if (model.RememberMe)
                {
                    Response.Cookies.Append("rememberMeToken", token, new CookieOptions() 
                    {
                        HttpOnly = true, 
                        Expires = tokenExpiration 
                    });
                }
                return Ok(new 
                { 
                    token = token,
                    username = user.Username,
                    isInstitution = user.IsInstitution,
                    needSetup = user.NeedSetup
                });
            }
            else
            {
                return Unauthorized();
            }
        }

        // institution area
        [Authorize]
        [HttpPost("Account/checkInstitutionName")]
        public async Task<IActionResult> CheckInstitutionName([FromBody] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // check that user is an institution
            var username = User.FindFirst("username")?.Value;
            var user = await dbContext.Users.FindAsync(username);

            if (user == null || user.IsInstitution == false)
            {
                return Unauthorized();
            }

            bool isNameAvailable = !this.dbContext.InstitutionsData.Any(i => i.InstitutionName == name);

            if (isNameAvailable)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("Account/institutionSetup/{institutionName}")]
        public async Task<IActionResult> InstitutionSetup(string institutionName, [FromBody] InstitutionSetupViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirst("username")?.Value;
            var user = await dbContext.Users.FindAsync(username);
            
            if (user == null || user.IsInstitution == false || user.NeedSetup == false)
            {
                return BadRequest();
            }
            var institutionData = await this.dbContext.InstitutionsData.FirstOrDefaultAsync(i => i.Username == username);
            if (institutionData == null)
            {
                // we should not be here
                return StatusCode(500);
            }

            // SETUP DATA:
            if (InstitutionData.BuildInputFromViewModel(institutionName, viewModel) == null)
            {
                return BadRequest();
            }
            institutionData.InstitutionName = institutionName;
            institutionData.SetupViewModelJsonFromObject(viewModel);
            // create form links
            institutionData.StaffFormId = $"{Guid.NewGuid()}";
            institutionData.StudentFormId = $"{Guid.NewGuid()}";
            // setup done
            user.NeedSetup = false;
            await dbContext.SaveChangesAsync();

            var newToken = RefreshToken(user);
            return Ok(new { token = newToken });
        }
        
        [Authorize]
        [HttpGet("Account/getFormsLinks")]
        public async Task<IActionResult> getFormsLinks()
        {
            var username = User.FindFirst("username")?.Value;
            var user = await dbContext.Users.FindAsync(username);

            if (user == null || user.IsInstitution == false || user.NeedSetup == true)
            {
                return Unauthorized();
            }

            var institutionData = await this.dbContext.InstitutionsData.FirstOrDefaultAsync(i => i.Username == username);
            if (institutionData == null)
            {
                // we should not be here
                return StatusCode(500);
            }

            if (institutionData.StudentFormId == null || institutionData.StaffFormId == null)
            {
                return BadRequest();
            }

            return Ok(new
            {
                staffFormId = institutionData.StaffFormId,
                studentFormId = institutionData.StudentFormId
            });
        }

        [HttpGet("checkFormId/{formId}")]
        public IActionResult CheckFormId(string formId)
        {
            var institutionData = dbContext.InstitutionsData.FirstOrDefault(i => i.StudentFormId == formId);
            if (institutionData != null)
            {
                return Ok( new 
                { 
                    formType = "student",
                    courseIds = institutionData.FromJson()?.CourseList?.Select(course => course.Id).ToList()
                });
            }

            institutionData = dbContext.InstitutionsData.FirstOrDefault(i => i.StaffFormId == formId);
            if (institutionData != null)
            {
                return Ok(new 
                { 
                    formType = "staff",
                    courseIds = institutionData.FromJson()?.CourseList?.Select(course => course.Id).ToList()
                });
            }
            return BadRequest();
        }

        [HttpPost("submitStaffForm/{formId}/{staffId}")]
        public IActionResult SubmitStaffForm(string formId, string staffId, [FromBody] List<Period> periods)
        {
            // search the form id
            var institutionData = dbContext.InstitutionsData.FirstOrDefault(item => item.StaffFormId == formId);
            if (institutionData == null)
            {
                return BadRequest();
            }
            InstitutionSetupViewModel? vm = institutionData.FromJson();
            if (vm == null)
            {
                return StatusCode(500);
            }
            if (vm.LoneUniStaffList == null || !vm.LoneUniStaffList.Any(item => item.Id == staffId))
            {
                return BadRequest();
            }
            var input = new StaffFormInput { StaffId = staffId, InstitutionUsername = institutionData.Username };
            input.SetUnavilableTimes(periods);
            this.dbContext.StaffFormInputs.Add(input);
            this.dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("submitStudentForm/{formId}/{studentId}")]
        public IActionResult SubmitStudentForm(string formId, string studentId, StudentFormViewModel viewModel)
        {
            // search the form id
            var institutionData = dbContext.InstitutionsData.FirstOrDefault(item => item.StudentFormId == formId);
            if (institutionData == null)
            {
                return BadRequest();
            }
            var input = new StudentFormInput
            {
                StudentId = studentId,
                InstitutionUsername = institutionData.Username,
            };
            input.SetUnavilableTimes(viewModel.UnavailableTimes);
            input.SetCourseIds(viewModel.CourseIds);
            this.dbContext.StudentFormInputs.Add(input);
            this.dbContext.SaveChangesAsync();
            return Ok();
        }

        [Authorize]
        [HttpPost("Account/callInstitutionSchedulingAlgo")]
        public async Task<IActionResult> CallInstitutionSchedulingAlgo()
        {
            // check that user exists as an institution and have done their setup
            var username = User.FindFirst("username")?.Value;
            var user = await dbContext.Users.FindAsync(username);

            if (user == null || user.IsInstitution == false || user.NeedSetup == true)
            {
                return Unauthorized();
            }

            // get institution's setup data
            var institutionData = await this.dbContext.InstitutionsData.FirstOrDefaultAsync(i => i.Username == username);
            if (institutionData == null)
            {
                // we should not be here
                return StatusCode(500);
            }
            InstitutionSetupViewModel? setupViewModel = institutionData.FromJson();
            if (setupViewModel == null)
            {
                return StatusCode(500);
            }
            InstitutionInput institutionInput = InstitutionData.BuildInputFromViewModel(institutionData.InstitutionName, setupViewModel);
            if (institutionInput == null)
            {
                return StatusCode(500);
            }
            List<StaffFormInput> staffForms = dbContext.StaffFormInputs.Where(item => item.InstitutionUsername == username).ToList();
            List<StudentFormInput> studentFormInputs = dbContext.StudentFormInputs.Where(item => item.InstitutionUsername == username).ToList();

            // set students
            List<Student> students;
            try
            {
                students = studentFormInputs.Select(item => item.ToStudent(institutionInput.CourseList)).ToList();
            } catch (Exception)
            {
                return StatusCode(500);
            }
            institutionInput.Students = students;

            // set staff prefrences
            List<LoneUniStaff> loneUniStaff = institutionInput.Staff.OfType<LoneUniStaff>().ToList();
            foreach (var form in staffForms)
            {
                var item = loneUniStaff.FirstOrDefault(s => s.ID == form.StaffId);
                if (item != null)
                {
                    var unavailableTimes = form.GetUnavilableTimes();
                    if (unavailableTimes != null)
                    {
                        item.UnavailableTimes = unavailableTimes;
                    }
                }
            }
            
            SchedulerAlgorithm schedulerAlgorithm = new SchedulerAlgorithm(institutionInput);
            schedulerAlgorithm.Run();
            
            bool isSuccessful;
            Dictionary<string, Dictionary<int, ScheduledCourseGroupData>> coursesScheduled;
            try
            {
                coursesScheduled = BuildInstitutionOutput(schedulerAlgorithm);
                if (schedulerAlgorithm.AlgorithmMessage == Constants.OverlapSuccess)
                {
                    isSuccessful = true;
                } else if (schedulerAlgorithm.AlgorithmMessage == Constants.OverlapFail)
                {
                    isSuccessful = false;
                } else
                {
                    return StatusCode(500);
                }
                institutionData.SetResults(coursesScheduled);
                institutionData.SuperCourses = schedulerAlgorithm.SuperCourses;
                InstitutionData.Results[institutionData.InstitutionName] = schedulerAlgorithm.SuperCourses;
            } catch (Exception)
            {
                return StatusCode(500);
            }

            // cleanup
            // remove old forms
            var staffFormsToRemove = dbContext.StaffFormInputs.Where(form => form.InstitutionUsername == username);
            dbContext.StaffFormInputs.RemoveRange(staffFormsToRemove);
            var studentFormsToRemove = dbContext.StudentFormInputs.Where(form => form.InstitutionUsername == username);
            dbContext.StudentFormInputs.RemoveRange(studentFormsToRemove);

            // generate new forms
            institutionData.StaffFormId = $"{Guid.NewGuid()}";
            institutionData.StudentFormId = $"{Guid.NewGuid()}";

            await dbContext.SaveChangesAsync();
            return Ok( new {output = coursesScheduled, isSuccessful = isSuccessful});
        }

        // Student area
        [Authorize]
        [HttpGet("Account/schedulesList")]
        public async Task<IActionResult> GetSchedulesList()
        {
            // check that user exists as a normal user
            var username = User.FindFirst("username")?.Value;
            var user = await dbContext.Users.FindAsync(username);

            if (user == null || user.IsInstitution == true)
            {
                return Unauthorized();
            }

            List<StudentAlgoResult> lst = dbContext.StudentAlgoResults.Where(r => r.Username == username).ToList();

            List<StudentScheduleItem> schedules = lst.Select(r => new StudentScheduleItem
            {
                Name = r.Name,
                Schedule = r.GetListResult(),
            }).ToList();

            return Ok(new { schedules = schedules });
        }

        // With Institution
        [Authorize]
        [HttpPost("Account/studentInputWithInstitution")]
        public IActionResult StudentInputWithInstitution(StudentInputWithInstitutionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // check that user exists as a normal user
            var username = User.FindFirst("username")?.Value;
            var user = dbContext.Users.Find(username);
            if (user == null || user.IsInstitution == true)
            {
                return Unauthorized();
            }

            // search the institution:
            InstitutionData institutionData = dbContext.InstitutionsData.FirstOrDefault(i => i.InstitutionName == viewModel.InstitutionName);
            if (institutionData == null)
            {
                return BadRequest();
            }

            // build input:
            List<string> wantedSemesters = new List<string>();
            if (viewModel.SemesterA)
            {
                wantedSemesters.Add("a");
            }
            if (viewModel.SemesterB)
            {
                wantedSemesters.Add("b");
            }
            if (viewModel.SemesterSummer)
            {
                wantedSemesters.Add("summer");
            }

            StudentDemands demands = new StudentDemands(viewModel.UnavilableTimes, viewModel.CourseIds, viewModel.LearningDays, viewModel.HoursPerDay, wantedSemesters);

            var superCourses = InstitutionData.Results[institutionData.InstitutionName];

            if (superCourses == null)
            {
                return BadRequest();
            }
            StudentsSchedulingAlgorithm algorithm = new StudentsSchedulingAlgorithm(demands, superCourses);
            algorithm.Run();
            Dictionary<Period, ScheduledStudentCourseGroupData> result = ParseStudentAlgoOutput(algorithm);

            List<StudentAlgorithmResultItemViewModel> listResult = result.Select(kv => new StudentAlgorithmResultItemViewModel
            {
                HourPeriod = kv.Key,
                GroupData = kv.Value
            }).ToList();

            StudentAlgoResult studentAlgoResult = new StudentAlgoResult
            {
                Id = username + "&" + viewModel.Name,
                Name = viewModel.Name,
                Username = username,
            };
            studentAlgoResult.SetListResult(listResult);
            dbContext.StudentAlgoResults.Add(studentAlgoResult);
            dbContext.SaveChanges();

            return Ok(new {result = listResult, name = viewModel.Name });
        }

        [Authorize]
        [HttpPost("Account/studentInputWithoutInstitution")]
        public IActionResult StudentInputWithoutInstitution(StudentInputWithoutInstitution viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // check that user exists as a normal user
            var username = User.FindFirst("username")?.Value;
            var user = dbContext.Users.Find(username);
            if (user == null || user.IsInstitution == true)
            {
                return Unauthorized();
            }

            // StudentDemands demands,
            // Dictionary<string, CourseProperties> courses

            List<string> wantedCourses = viewModel.CoursesData.Select(c => c.Id).ToList();

            List<string> wantedSemesters = new List<string>();
            if (viewModel.SemesterA)
            {
                wantedSemesters.Add("a");
            }
            if (viewModel.SemesterB)
            {
                wantedSemesters.Add("b");
            }
            if (viewModel.SemesterSummer)
            {
                wantedSemesters.Add("summer");
            }

            StudentDemands demands = new StudentDemands(viewModel.UnavilableTimes, wantedCourses, viewModel.LearningDays, viewModel.HoursPerDay, wantedSemesters);

            Dictionary<string, CourseProperties> courses = new Dictionary<string, CourseProperties>();

            foreach (CourseDataViewModel courseVM in viewModel.CoursesData)
            {
                string key = courseVM.Id;

                // course propeties
                string name = courseVM.Name;
                string id = courseVM.Id;

                // groups
                Dictionary<string, Dictionary<int, List<Period>>> groups = new Dictionary<string, Dictionary<int, List<Period>>>();
                Dictionary<int, List<Period>> lectureGroups = new Dictionary<int, List<Period>>();
                Dictionary<int, List<Period>> exreciseGroups = new Dictionary<int, List<Period>>();

                int i = 0;
                foreach (Period period in courseVM.LectureTimes)
                {
                    int groupNumber = i;
                    List<Period> value = new List<Period> { period };
                    lectureGroups[groupNumber] = value;
                    i++;
                }

                i = 0;
                foreach (Period period in courseVM.ExreciseTimes)
                {
                    int groupNumber = i;
                    List<Period> value = new List<Period> { period };
                    exreciseGroups[groupNumber] = value;
                    i++;
                }
                groups[Constants.Lecture] = lectureGroups;
                groups[Constants.Exercise] = exreciseGroups;

                courses[key] = new CourseProperties(name, id, groups, courseVM.LectureDuration, courseVM.ExreciseDuration);
            }

            List<StudentAlgorithmResultItemViewModel> listResult;
            try
            {
                StudentsSchedulingAlgorithm algorithm = new StudentsSchedulingAlgorithm(demands, courses);
                algorithm.Run();
                Dictionary<Period, ScheduledStudentCourseGroupData>  result = ParseStudentAlgoOutput(algorithm);
                listResult = result.Select(kv => new StudentAlgorithmResultItemViewModel
                {
                    HourPeriod = kv.Key,
                    GroupData = kv.Value
                }).ToList();
            } catch (Exception)
            {
                return BadRequest();
            }

            StudentAlgoResult studentAlgoResult = new StudentAlgoResult
            {
                Id = username + "&" + viewModel.Name,
                Name = viewModel.Name,
                Username = username,
            };
            studentAlgoResult.SetListResult(listResult);
            dbContext.StudentAlgoResults.Add(studentAlgoResult);
            dbContext.SaveChanges();

            return Ok(new {result = listResult, name = viewModel.Name });
        }

        [Authorize]
        [HttpGet("Account/finishedInstitutionNames")]
        public IActionResult finishedInstitutionNames() 
        {
            List<string> names = new List<string>();

            foreach(InstitutionData institutionData in dbContext.InstitutionsData)
            {
                names.Add(institutionData.InstitutionName);
            }

            return Ok(new { names });
        }

        private Dictionary<Period, ScheduledStudentCourseGroupData> ParseStudentAlgoOutput(StudentsSchedulingAlgorithm algorithm)
        {
            var schedules = algorithm.ProStudentScheduling.Schedule;
            var parsedData = new Dictionary<Period, ScheduledStudentCourseGroupData>();
            ScheduledStudentCourseGroupData defaultParsedData = new ScheduledStudentCourseGroupData 
            {
                IsEmpty = true,
                CourseId = "-1",
                CourseName = "-1",
                GroupNumber = -1,
                LessonType = "-1"
            };
            ScheduledStudentCourseGroupData groupedCourseData = defaultParsedData;
            foreach (Period periodKey in Constants.AvailablePeriods)
            {
                bool found = false;
                foreach (KeyValuePair<Period, (string, int, CourseProperties)> kvp in schedules)
                {
                    var coursePeriod = kvp.Key;
                    if (coursePeriod.Day == periodKey.Day &&
                        coursePeriod.Semester == periodKey.Semester &&
                        coursePeriod.StartTime <= periodKey.StartTime &&
                        coursePeriod.EndTime >= periodKey.EndTime)
                    {
                        groupedCourseData = new ScheduledStudentCourseGroupData
                        {
                            IsEmpty = false,
                            CourseId = kvp.Value.Item3.ID,
                            CourseName = kvp.Value.Item3.Name,
                            GroupNumber = kvp.Value.Item2,
                            LessonType = kvp.Value.Item1
                        };
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    parsedData[periodKey] = groupedCourseData;
                }
                else
                {
                    parsedData[periodKey] = defaultParsedData;
                }
            }
            return parsedData;
        }

        private Dictionary<string, Dictionary<int, ScheduledCourseGroupData>> BuildInstitutionOutput(SchedulerAlgorithm schedulerAlgorithm)
        {
            var coursesScheduled = new Dictionary<string, Dictionary<int, ScheduledCourseGroupData>>();
            foreach (KeyValuePair<Course, CourseScheduling> kvp in schedulerAlgorithm.SuperCourses)
            {
                string? newKey = kvp.Key.CourseId;
                if (newKey == null)
                {
                    throw new Exception();
                }
                Dictionary<int, ScheduledCourseGroupData> newValue = ParseCourseScheduling(kvp.Value);
                coursesScheduled[newKey] = newValue;
            }
            return coursesScheduled;
        }

        private Dictionary<int, ScheduledCourseGroupData> ParseCourseScheduling(CourseScheduling courseScheduling)
        {
            var parsedCourseScheduling = new Dictionary<int, ScheduledCourseGroupData>();
            Dictionary<int, (UniStaff, List<Period>, string)> courseGroups = courseScheduling.CourseGroups;

            foreach (KeyValuePair<int, (UniStaff, List<Period>, string)> kvp in courseGroups)
            {
                int groupIdKey = kvp.Key;
                
                UniStaff staff = kvp.Value.Item1;
                List<string> staffIdList = new List<string>();
                if (staff is LoneUniStaff)
                {
                    LoneUniStaff loneUniStaff = (LoneUniStaff)staff;
                    staffIdList.Add(loneUniStaff.ID);
                } else
                {
                    MultipleUniStaff multipleUniStaff = (MultipleUniStaff)staff;
                    List<UniStaff> uniStaffList = multipleUniStaff.UniStaffList;
                    foreach (UniStaff uniStaff in uniStaffList)
                    {
                        LoneUniStaff innerStaff = (LoneUniStaff)uniStaff;
                        staffIdList.Add(innerStaff.ID);
                    }
                }
                List<Period> periods = kvp.Value.Item2;
                string role = kvp.Value.Item3;

                parsedCourseScheduling[groupIdKey] = new ScheduledCourseGroupData 
                { 
                    StaffIds = staffIdList,
                    Periods = periods,
                    Type = role 
                };
            }
            return parsedCourseScheduling;
        }

        private string RefreshToken(User user)
        {
            var currentExpiration = User.FindFirst("exp")?.Value;
            // Parse the current expiration date
            var expiration = DateTime.UnixEpoch.AddSeconds(long.Parse(currentExpiration));
            return GenerateJWTToken(user, expiration);
        }
   
        private string GenerateJWTToken(User user, DateTime expiration)
        {
            // Define secret key and issuer
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyManager.GetSecretKey()));
            var issuer = "Scheduler-Pro";

            // Create claims for the token
            var claims = new[]
            {
                new Claim("username", user.Username),
                new Claim("isInstitution", user.IsInstitution ? "true" : "false"),
                new Claim("isSetupNeeded", user.NeedSetup ? "true" : "false")
            };

            var token = new JwtSecurityToken (
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: expiration,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            // Return token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
