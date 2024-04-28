using FMInatorul.Data;
using FMInatorul.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FMInatorul.Controllers
{
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

        public IActionResult Index()
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
                return View("UploadPdf", responseString); // Return the API response string
            }
            else
            {
                // Handle API upload error
                return StatusCode((int)response.StatusCode, $"Error uploading file: {response.ReasonPhrase}");
            }
        }

        [HttpGet]
        public IActionResult QuizResult([FromBody] QuizModel quizModel)
        {
            if (quizModel != null)
            {
                var totalQuestions = quizModel.Questions.Count;
                var correctAnswers = 0;

                foreach (var question in quizModel.Questions)
                {
                    if (question.Answer == question.SelectedAnswer)
                    {
                        correctAnswers++;
                    }
                }

                float score = correctAnswers * 100 / totalQuestions;
                ViewBag.Score = score;
                return View();
            }
            else
            {
                return RedirectToAction("Error");
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
    }
}
