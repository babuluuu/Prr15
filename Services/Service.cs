using Microsoft.EntityFrameworkCore;
using Learning_App.Models;
using Learning_App.Models.Request;
using Learning_App.Models.Response;
using System.Text;

namespace Learning_App.Services
{
    public class Service
    {
        private DatabaseContext _context;
        public Service(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> SignUp(SignUpRequest requestModel)
        {
            try
            {
                User user = new User()
                {
                    Email = requestModel.Email,
                    FirstName = requestModel.FirstName,
                    LastName = requestModel.LastName,
                    Password = Encoding.UTF8.GetBytes(BCrypt.Net.BCrypt.HashPassword(requestModel.Password)),
                    Role = requestModel.Role,
                };

                await _context.Users.AddAsync(user);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<User?> Login(LoginRequest requestModel)
        {
            try
            {
                var user = await _context.Users
                .Where(x => x.Email == requestModel.Email )
                .FirstOrDefaultAsync();

                if(user == null)
                {
                    return null;
                }
             
                if(!BCrypt.Net.BCrypt.Verify(requestModel.Password, Encoding.UTF8.GetString(user.Password)))
                {
                    return null;
                }
                return user;
            }
            catch (Exception e)
            {
                return null;
            }
            
        }

        public async Task<List<ListUserResponse>> GetUserList()
        {
            try
            { 
                var users = await _context.Users // users is List<User>
                .Select(x => new ListUserResponse() // x is User
                {
                    UserId = x.UserId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Role = x.Role,
                })
                .ToListAsync(); 

                return users; 
            }
            catch (Exception e)
            {
                return null;
            }
        } 
        
        public async Task<bool> DeleteUser(int userId)
        {
            try
            {
                var user = await _context.Users
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

                if(user == null)
                {
                    return false;
                }

                _context.Users.Remove(user);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        } 

        public async Task<bool> CreateCourse(Course course, List<Lesson> Lessons, List<CourseResource> Resources, List<CourseAssignments> Assignments)
        {

            try
            {
                var newCourse = new Course()
                {
                    Description = course.Description,
                    ImageUrl = course.ImageUrl,
                    UserId = course.UserId,
                    Title = course.Title,
                };

                await _context.Courses.AddAsync(newCourse);

                await _context.SaveChangesAsync();

                List<int> lessonIds = new List<int>();


                foreach (var lesson in Lessons)
                {
                    var newLesson = new Lesson()
                    {
                        Course = newCourse,
                        CourseId = newCourse.CourseId,
                        Description = lesson.Description,
                        Title = lesson.Title,
                        Resources = new List<CourseResource>(),
                    };

                    await _context.Lessons.AddAsync(newLesson); // lesson is Lesson
                    await _context.SaveChangesAsync(); // lesson.LessonId is now available
                    lessonIds.Add(newLesson.LessonId); // lessonIds is List<int>
                }

                for (int i = 0; i < Resources.Count; i++)
                {
                    var resource = Resources[i];
                    var newResource = new CourseResource()
                    {
                        CourseId = newCourse.CourseId,
                        LessonId = lessonIds[i],
                        ContentInfo = resource.ContentInfo,
                        ResourceType = resource.ResourceType,
                    };

                    await _context.CourseResources.AddAsync(newResource);
                    await _context.SaveChangesAsync();

                }

                foreach (var assignment in Assignments)
                {
                    var newAssignment = new CourseAssignments()
                    {
                        CourseId = newCourse.CourseId,
                        Description = assignment.Description,
                        OverallScore = 0,
                        Title = assignment.Title,
                    };

                    await _context.CourseAssignments.AddAsync(newAssignment); // lesson is Lesson
                    await _context.SaveChangesAsync(); // lesson.LessonId is now available
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            

            
        }
        
        //tunahan-bas
        public async Task<List<ListCourses>> GetCoursesList(bool isStudent, int? userId)
        {
            try
            {
                var enrolledCourseIds = new List<int>();
                if(isStudent && userId != null)
                {
                    enrolledCourseIds = await _context.CourseEnrollment
                        .Where(x => x.StudentId == userId.Value)
                        .Select(x => x.CourseId).ToListAsync();
                }

                var courses= await _context.Courses // users is List<User>
                .Include(x => x.Instructor)
                .Select(x => new ListCourses() // x is User
                {
                    CourseId = x.CourseId,
                    Title = x.Title,
                    Description = x.Description,
                    Instructor = new ListCourses.InstructorInfo()
                    {
                        Name = x.Instructor.FirstName + " " + x.Instructor.LastName,
                    },
                    ImageUrl = x.ImageUrl,
                    IsEnrolled = isStudent ? enrolledCourseIds.Contains(x.CourseId) : false,
                })
                .ToListAsync(); 

                return courses; 
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<bool> DeleteCourse(int courseId)
        {
            try
            {
                var course = await _context.Courses
                .Where(x => x.CourseId == courseId)
                .FirstOrDefaultAsync();

                if(course == null)
                {
                    return false;
                }

                _context.Courses.Remove(course);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        //tunahan-bit
        //tunahan-bas2
        public async Task<List<ListAssignments>> GetAssignmentList()
        {
            try
            { 
                var assignments= await _context.StudentCourseAssignments // users is List<User>
                    
                .Select(x => new ListAssignments() // x is User
                {
                    StudentCourseAssignmentId = x.StudentCourseAssignmentId,
                    UserId = x.UserId,
                    AssignmentId = x.AssignmentId,
                    Score = x.Score,
                    FileUrl = x.FileUrl,
                })
                .ToListAsync(); 

                return assignments; 
            }
            catch (Exception e)
            {
                return null;
            }
        } 
        //tunahan-bit2
        public async Task<bool> EnrollCourse(int courseId, int studentId)
        {
            await _context.CourseEnrollment.AddAsync(new CourseEnrollment()
            {
                CourseId = courseId,
                Grade = 0,
                StudentId = studentId,
            });

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Models.Response.DetailedCourseInfo> GetCourseDetail(int courseId, int userId)
        {
            try
            {
                var userSubmittedAssignmentIds = await _context.StudentCourseAssignments
                    .Where(x => x.UserId == userId)
                    .Select(x => x.AssignmentId)
                    .ToListAsync();

                var grade = await _context.CourseEnrollment
                    .Where(x => x.StudentId == userId && x.CourseId == courseId)
                    .Select(x => x.Grade)
                    .FirstAsync();

                //var score = await _context.StudentCourseAssignments 
                //    .Where(x => x.UserId == userId && x.AssignmentId
                var courseAssignmentIds = await _context.Courses.Include(x => x.Assignments)
                    .Where(x => x.CourseId==courseId)
                    .Select(x => x.Assignments.Select(y=>y.AssignmentId)).FirstAsync();

                var scores = _context.StudentCourseAssignments
                        .Where(y => y.UserId == userId && courseAssignmentIds.Contains(y.AssignmentId))
                        .Select(x => x.Score).ToList();
               
                double score = 0;
                if (scores.Count > 0)
                {
                    
                     score = scores.Sum(x => x) / scores.Count;
                }


                var course = await _context.Courses
                    .Include(x => x.Assignments)
                    .Include(x => x.Lessons)
                    .ThenInclude(x => x.Resources)
                    .Where(x => x.CourseId == courseId)
                    .Select(x => new Models.Response.DetailedCourseInfo()
                    {
                        CourseId = x.CourseId,
                        Title = x.Title,
                        Description = x.Description,
                        ImageUrl = x.ImageUrl,
                        Grade = grade,
                        Score = score,
                        Lessons = x.Lessons.Select(y => new DetailedCourseInfo.LessonInfo()
                        {
                            LessonId = y.LessonId,
                            Title = y.Title,
                            Description = y.Description,
                            Resource = new DetailedCourseInfo.ResourceInfo()
                            {
                                ResourceId = y.Resources.First().ResourceId,
                                Type = y.Resources.First().ResourceType,
                                Content = y.Resources.First().ContentInfo,
                            }
                        }).ToList(),
                        Assignments = x.Assignments.Select(y => new DetailedCourseInfo.AssignmentInfo()
                        {
                            AssignmentId = y.AssignmentId,
                            Title = y.Title,
                            Description = y.Description,
                            IsSubmitted = userSubmittedAssignmentIds.Contains(y.AssignmentId),
                        }).ToList(),
                    }).FirstAsync();

                return course;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public async Task<bool> SubmitAssignment(int assignmentId,int userId ,string filePath)
        {

            try
            {
                var newAssignment = new StudentCourseAssignment()
                {
                    AssignmentId = assignmentId,
                    UserId = userId,
                    FileUrl = filePath,
                    Score = 0,
                };

                var courseId = await _context.CourseAssignments
                    .Where(x => x.AssignmentId == assignmentId)
                    .Select(x => x.CourseId)
                    .FirstAsync();

                await _context.StudentCourseAssignments.AddAsync(newAssignment);

                await _context.SaveChangesAsync();

                await UpdateGrade(userId, courseId);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }   
        }


        public async Task<bool> NextLesson(int lessonId, int userId,int courseId)
        {

            try
            {
                var SubmittedAssignmentIds = await _context.StudentCourseAssignments
                    .Where(x => x.UserId == userId)
                    .Select(x => x.AssignmentId)
                    .ToListAsync();

               

                var CourseInfos = await _context.Courses
                    .Include(x => x.Lessons)
                    .Include(x => x.Assignments)
                    .Where(x => x.CourseId == courseId)
                    .Select(x =>new 
                    {
                        LessonIds = x.Lessons.OrderBy(y => y.LessonId).Select(y => y.LessonId).ToList(),
                        AssignmentInfos = x.Assignments.OrderBy(y => y.AssignmentId).Select(y => new
                        {
                            Id = y.AssignmentId,
                            IsSubmitted = SubmittedAssignmentIds.Contains(y.AssignmentId),

                        }
                        ).ToList(),

                    })
                    .FirstAsync();


                
                var Enrollment = await _context.CourseEnrollment
                    .Where(x => x.StudentId == userId && x.CourseId == courseId)
                    .FirstAsync();

                Enrollment.LastCompletedLessonId = lessonId;

                _context.CourseEnrollment.Update(Enrollment);
                await _context.SaveChangesAsync();

                await UpdateGrade(userId, courseId);  

                return true;


            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task UpdateGrade(int userId,int courseId)
        {
            try
            {
                var SubmittedAssignmentIds = await _context.StudentCourseAssignments
                   .Where(x => x.UserId == userId)
                   .Select(x => x.AssignmentId)
                   .ToListAsync();

                var CourseInfos = await _context.Courses
                    .Include(x => x.Lessons)
                    .Include(x => x.Assignments)
                    .Where(x => x.CourseId == courseId)
                    .Select(x => new
                    {
                        LessonIds = x.Lessons.OrderBy(x => x.LessonId).Select(y => y.LessonId).ToList(),
                        AssignmentInfos = x.Assignments.OrderBy(x => x.AssignmentId).Select(y => new
                        {
                            Id = y.AssignmentId,
                            IsSubmitted = SubmittedAssignmentIds.Contains(y.AssignmentId),

                        }
                        ).ToList(),

                    })
                    .FirstAsync();


                var point = 100 / (CourseInfos.LessonIds.Count + CourseInfos.AssignmentInfos.Count);

                var Enrollment = await _context.CourseEnrollment
                    .Where(x => x.StudentId == userId && x.CourseId == courseId)
                    .FirstAsync();

                var LessonPoint = 0;

                if(Enrollment.LastCompletedLessonId != null)
                {
                    var CurrentIndex = CourseInfos.LessonIds.IndexOf(Enrollment.LastCompletedLessonId.Value);
                    LessonPoint = point * (CurrentIndex + 1);
                }
                
                var AssignmentPoint = CourseInfos.AssignmentInfos.Where(x => x.IsSubmitted).Count() * point;
                Enrollment.Grade = LessonPoint + AssignmentPoint;
                _context.CourseEnrollment.Update(Enrollment);
                await _context.SaveChangesAsync();
            }
             catch(Exception e)
            {
                return ;
            }
        }
    }
}
