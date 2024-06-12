using FMInatorul.Data;
using FMInatorul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Diagnostics;

namespace FMInatorul.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
    {
        _db = context;
        _logger = logger;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult IndexNew()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Admin()
    {
        if (User.IsInRole("Admin"))
        {
            var Materii = _db.Materii;
            ViewBag.materii = Materii;
            return View();
        }
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    public IActionResult UploadAdminPdf()
    {
        var materie_id = Convert.ToInt32(HttpContext.Request.Query["materie"]);
        ViewBag.materie = _db.Materii.Find(materie_id);
        
        ViewBag.materie_id = materie_id;

        return View();
    }

    public IActionResult Add_Questions(JsonContent questions)
    {
        var materie_id = Convert.ToInt32(HttpContext.Request.Query["materie"]);
        ViewBag.materie = _db.Materii.Find(materie_id);
        //add the questions to the database directly from the json


        
        return View(questions);
    }

    [HttpPost]
    public async Task<IActionResult> Add_Questions([FromForm] IFormFile file)
    {
        var materie_id = Convert.ToInt32(HttpContext.Request.Query["materie"]);
        ViewBag.materie = _db.Materii.Find(materie_id);

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

            // Print the response JSON to the console
            Console.WriteLine(responseString);

            //var questions = await response.Content.ReadFromJsonAsync<QuestionDtoList>();
            //var questions = await response.Content.ReadAsStringAsync();

            var jsonString = @"
            {
                ""Questions"": [
                {
                    ""answer"": ""A"",
                    ""choices"": {
                    ""A"": ""Option A"",
                    ""B"": ""Option B"",
                    ""C"": ""Option C"",
                    ""D"": ""Option D""
                    },
                    ""question"": ""What is the capital of France?""
                },
                {
                    ""answer"": ""C"",
                    ""choices"": {
                    ""A"": ""London"",
                    ""B"": ""Berlin"",
                    ""C"": ""Paris"",
                    ""D"": ""Rome""
                    },
                    ""question"": ""Which city is known as the City of Love?""
                }
                ]
            }";

            var questions = JsonConvert.DeserializeObject<QuestionDtoList>(jsonString);

 
            
            foreach (var questionDto in questions.Questions)
            {
                var intrebareRasp = new IntrebariRasp
                {
                    intrebare = questionDto.Question,
                    raspunsCorect = questionDto.Answer,
                    validareProfesor = 0, // Assuming a default value; adjust as needed
                    MaterieId = materie_id,
                    Variante = questionDto.Choices.Select(choice => new Variante
                    {
                        Choice = choice.Value,
                        VariantaCorecta = choice.Key == questionDto.Answer ? 1 : 0
                    }).ToList()
                };

                _db.IntrebariRasps.Add(intrebareRasp);
            }

            _db.SaveChanges();
            
            ViewBag.materie = _db.Materii.Find(materie_id);

            return View("Add_Questions"); // Pass the quiz model to the view
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

    //[HttpPost]
    //public IActionResult Add_Questions()
    //{

    //}

}