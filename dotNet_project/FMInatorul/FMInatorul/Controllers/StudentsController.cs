using FMInatorul.Data;
using FMInatorul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FMInatorul.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ApplicationDbContext db;
        private object form;

        public StudentsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
       
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var student = await db.Students
                                    .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if(student.CompletedProfile == false )
            {
                if (TempData.ContainsKey("ErrorMessage"))
                {
                    ViewBag.Message = TempData["ErrorMessage"];
                    return View(user);
                }
                else
                {
                    TempData["ErrorMessage"] = "Please finish completing your profile.";
                    ViewBag.Message = TempData["ErrorMessage"];
                    return View(user);
                }
            }

            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.Message = TempData["SuccessMessage"];
                return View(user);
            }
            return View(user);
        }

        public IActionResult Play()
        {
            return View();
        }

        public IActionResult UploadPdf()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadPdf([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded!");
            }

            if (!IsPdfFile(file))
            {
                return BadRequest("Invalid file format. Only PDF files allowed!");
            }

            // Upload the PDF to your Flask API (replace with your actual API URL)
            //http://46.101.136.24/
            var response = await UploadPdfToFlaskApiAsync(file, "http://46.101.136.24:5555/");

            if (response.IsSuccessStatusCode)
            {
                // Get the string response from the API
                var responseString = await response.Content.ReadAsStringAsync();
                var quiz = JsonConvert.DeserializeObject<QuizModel>(responseString); // Deserialize the JSON response

                Debug.WriteLine($"Response String: {responseString}");

                return View("Quiz", quiz); // Pass the quiz model to the view
            }
            else
            {
                // Handle API upload error
                return StatusCode((int)response.StatusCode, $"Error uploading file: {response.ReasonPhrase}");
            }
        }
        
        private bool IsPdfFile(IFormFile file)
        {
            return file.ContentType.Contains("application/pdf");
        }

        private async Task<HttpResponseMessage> UploadPdfToFlaskApiAsync(IFormFile file, string url)
        {
            using (var client = new HttpClient())
            {
                using (var multipartContent = new MultipartFormDataContent())
                {
                    using (var fileStream = file.OpenReadStream())
                    {
                        multipartContent.Add(new StreamContent(fileStream), "file", Path.GetFileName(file.FileName));
                        var response = await client.PostAsync(url, multipartContent);
                        return response;
                    }
                }
            }
        }

        public ActionResult ChatView()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult SubmitQuiz(QuizModel quiz)
        {
            return View("Results", quiz); // You could also pass the entire model to show detailed results
        }


        public IActionResult MateriiSingle()
        {
            var Materii = db.Materii.ToList();
            ViewBag.materii = Materii;
            return View();
        }

        public async Task<IActionResult> EditYear()
        {
            var userId = _userManager.GetUserId(User);
            var student = await db.Students
                                        .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (student == null)
            {
                return NotFound();
            }
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.Message = TempData["ErrorMessage"];
                return View(student);
            }
            return View(student);
        }

        [HttpPost]
        public IActionResult EditYear(int id, Student student)
        {
            if (ModelState.IsValid)
            {
                return View(student);
            }
            else
            {
                Student studentToUpdate = db.Students.Where(stu => stu.Id == id).First();

                if (studentToUpdate == null)
                {
                    return NotFound();
                }

                studentToUpdate.Year = student.Year;
                if (studentToUpdate.Year != 0 && studentToUpdate.Semester != 0)
                {
                    studentToUpdate.CompletedProfile = true;
                }
                if (student.Year >= 4 || student.Year <= 0)
                {
                    TempData["ErrorMessage"] = "Year must be between 1 and 3.";
                    return RedirectToAction("EditYear", "Students");
                }
                db.SaveChanges();
                TempData["SuccessMessage"] = "Year updated successfully.";
                return RedirectToAction("Index", "Students");
            }

        }

        public async Task<IActionResult> EditSemester()
        {
            var userId = _userManager.GetUserId(User);
            var student = await db.Students
                                        .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (student == null)
            {
                return NotFound();
            }
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.Message = TempData["ErrorMessage"];
                return View(student);
            }
            return View(student);
        }

        [HttpPost]
        public IActionResult EditSemester(int id, Student student)
        {
            if (ModelState.IsValid)
            {
                return View(student);
            }
            else
            {

                Student studentToUpdate = db.Students.Where(stu => stu.Id == id).First();

                if (studentToUpdate == null)
                {
                    return NotFound();
                }

                studentToUpdate.Semester = student.Semester;
                if (studentToUpdate.Year != 0 && studentToUpdate.Semester != 0)
                {
                    studentToUpdate.CompletedProfile = true;
                }
                if(student.Semester >=3 || student.Semester <=0)
                {
                    TempData["ErrorMessage"] = "Semester must be between 1 and 2.";
                    return RedirectToAction("EditSemester", "Students");
                }
                db.SaveChanges();
                TempData["SuccessMessage"] = "Semester updated successfully.";
                return RedirectToAction("Index", "Students");
            }

        }

    }
}
