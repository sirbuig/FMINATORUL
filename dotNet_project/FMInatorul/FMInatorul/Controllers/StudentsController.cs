using FMInatorul.Data;
using FMInatorul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

            return View(user);
        }

        public IActionResult Play()
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
        
        [HttpPost]
        public IActionResult SubmitQuiz(QuizModel quiz)
        {
            return View("Results", quiz); // You could also pass the entire model to show detailed results
        }
    }
}
