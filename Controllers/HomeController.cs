using System;
using System.Diagnostics;
using Learning_App.Models;
using Learning_App.Models.Response;
using Learning_App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Learning_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private DatabaseContext _context;
        private readonly Service _service;
        private readonly IWebHostEnvironment _env;


        public HomeController(ILogger<HomeController> logger, DatabaseContext context, Service service, IWebHostEnvironment environment)
        {
            _logger = logger;
            _context = context;
            _service = service;
            _env = environment;

        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetInt32("user_role") ?? -1;
            var userId = HttpContext.Session.GetInt32("user_id");

            bool isStudent = false;
            if(role == 1)
            {
                isStudent = true;
            }

            var courses = await _service.GetCoursesList(isStudent, userId);
            return View(courses);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> AdminView()
        {
            var users = await _service.GetUserList();
            return View(users);
        }

        public IActionResult CreateUserView()
        {
            return View();
        }

        public IActionResult CreateCourse()
        {
            return View();
        }

        public async Task<IActionResult> CourseDetailView(int courseId)
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            var course = await _service.GetCourseDetail(courseId, userId.Value);

            return View(course);
        }

        [Route("signUpAction")]
        [HttpPost]
        public async Task<IActionResult> SignUpAction(string firstName, string lastName, string email, string password)
        {
            var response = await _service.SignUp(new Models.Request.SignUpRequest()
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Password = password,
                Role = UserRoleTypeEnum.STUDENT,
            });

            if(response)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.error = "Server error!";
                return Ok();
            }
        }


        [Route("loginAction")]
        [HttpPost]
        public async Task<IActionResult> LoginAction(string email, string password)
        {
            var user = await _service.Login(new Models.Request.LoginRequest()
            {
                Email = email,
                Password = password,
            });

            if (user != null)
            {
                HttpContext.Session.SetInt32("user_id", user.UserId);
                HttpContext.Session.SetString("user_name", user.FirstName + " " + user.LastName);
                HttpContext.Session.SetString("user_email", user.Email);
                HttpContext.Session.SetInt32("user_role", (int)user.Role);

                switch (user.Role)
                {
                    case UserRoleTypeEnum.STUDENT:
                        return RedirectToAction("Index");
                    case UserRoleTypeEnum.INSTRUCTOR:
                        return RedirectToAction("Index");
                    case UserRoleTypeEnum.ADMIN:
                        return RedirectToAction("Index");
                    default:
                        ViewBag.error = "Geçersiz giriş";
                        return View();
                }
            }
            else
            {
                ViewBag.error = "Email veya şifre yanlış";
                return View();
            }
        }


        [Route("DeleteUser")]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int UserId)
        {
            var response = await _service.DeleteUser(UserId);

            if (response)
            {
                return RedirectToAction("AdminView");
            }
            else
            {
                ViewBag.error = "Server error!";
                return Ok();
            }   
        }

        [Route("createUserAction")]
        [HttpPost]
        public async Task<IActionResult> CreateUserAction(string firstName, string lastName, string email, string password, int role)
        {
            var response = await _service.SignUp(new Models.Request.SignUpRequest()
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Password = password,
                Role = (UserRoleTypeEnum) role,
            });

            if (response)
            {
                return RedirectToAction("AdminView");
            }
            else
            {
                ViewBag.error = "Server error!";
                return Ok();
            }
        }
        //tunahan-bas
        
        public async Task<IActionResult> InstructorView()
        {
            var course = await _service.GetCoursesList(false, null);
            return View(course);
        }
        
        [Route("DeleteCourse")]
        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int CourseId)
        {
            var response = await _service.DeleteCourse(CourseId);

            if (response)
            {
                return RedirectToAction("InstructorView");
            }
            else
            {
                ViewBag.error = "Server error!";
                return Ok();
            }   
        }
        
        //tunahan-bit

        
        //tunahan-bas2
        public async Task<IActionResult> SubmitedAssignments()
        {
            var assignment = await _service.GetAssignmentList();
            return View(assignment); 
        }
        
        [HttpPost]
        public IActionResult UpdateScore(int assignmentId, string instructorInput)
        {
            try
            {
                // assignmentId kullanarak ilgili görevi bul
                var assignment = _context.StudentCourseAssignments.FirstOrDefault(a => a.StudentCourseAssignmentId == assignmentId);

                if (assignment != null)
                {
                    // instructorInput kullanarak Score'u güncelle
                    assignment.Score = float.Parse(instructorInput); // Örnek olarak string'i float'a çeviriyoruz.

                    // Veritabanında değişiklikleri kaydet
                    _context.SaveChanges();
                }

                // Güncelleme işleminden sonra, sayfayı tekrar yükle veya başka bir işlem yapabilirsiniz.
                var updatedAssignments = _service.GetAssignmentList().Result; // Yeniden veri almak için
                return View("SubmitedAssignments", updatedAssignments);
            }
            catch (Exception ex)
            {
                // Hata yönetimi yapmak istediğiniz yer
                return RedirectToAction("Error");
            }
        }


        //tunahan-bit2

        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Route("createCourseAction")]
        [HttpPost]
        public async Task<IActionResult> CreateCourseAction(Course course, List<Lesson> Lessons, List<CourseResource> Resources, List<CourseAssignments> assignments, IFormFile ImageFile)
        {
            // Kursu veritabanına kaydetme işlemi burada yapılır
            // course nesnesi veritabanına eklenir ve kaydedilir

            int userId = HttpContext.Session.GetInt32("user_id" ) ?? 0; 
            course.UserId = userId;
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Dosya yükleme işlemi
                var uploads = Path.Combine(_env.WebRootPath, "course_thumbnails"); // Dosyanın yükleneceği klasör
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName; // Dosya adını benzersizleştirme

                var filePath = Path.Combine(uploads, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                // Dosya yükleme işlemi tamamlandığında dosyanın adını modeldeki ImageUrl'e kaydet
                course.ImageUrl = "/course_thumbnails/" + uniqueFileName;
            }

            var response = await _service.CreateCourse(course, Lessons, Resources, assignments);
            // yorum satırı ekledim
            
            if(response)
            {
                
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("CreateCourse");
            }   

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login"); 
        }

        public async Task<IActionResult> EnrollCourse(int courseId)
        {
            var studentId = HttpContext.Session.GetInt32("user_id") ?? -1;

            await _service.EnrollCourse(courseId, studentId);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> UploadAssignment(IFormFile file, int assignmentId, int courseId)
        {
            var userId = HttpContext.Session.GetInt32("user_id") ?? -1;

            if (file != null && file.Length > 0)
            {
                // Dosya yükleme işlemi
                var uploads = Path.Combine(_env.WebRootPath, "assignment_files"); // Dosyanın yükleneceği klasör
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName; // Dosya adını benzersizleştirme

                var filePath = Path.Combine(uploads, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                await _service.SubmitAssignment(assignmentId, userId, "/assignment_files/" + uniqueFileName);

            }

            return RedirectToAction("CourseDetailView", new { courseId = courseId });


        }


        public async Task<ActionResult> NextLesson(int courseId, int lessonId)
        {
            var userId = HttpContext.Session.GetInt32("user_id") ?? -1;

            await _service.NextLesson(lessonId,userId,courseId);

            return RedirectToAction("CourseDetailView", new { courseId = courseId });



        }

    }
}