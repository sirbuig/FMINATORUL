using FMInatorul.Data;
using FMInatorul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

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

	// Returns the main view of the application.
	public IActionResult Index()
	{
		return View();
	}

	// Returns a new version of the index view.
	public IActionResult IndexNew()
	{
        return View();
	}

	// Returns the privacy view.
	public IActionResult Privacy()
	{
		return View();
	}

	// Returns the error view with error details.
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}

	// Returns the admin view if the user is an admin.
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

	// Returns the view to upload a PDF for admins.
	[Authorize(Roles = "Admin")]
	public IActionResult UploadAdminPdf()
	{
		var materie_id = Convert.ToInt32(HttpContext.Request.Query["materie"]);
		ViewBag.materie = _db.Materii.Find(materie_id);
		ViewBag.materie_id = materie_id;

		return View();
	}

	// Adds questions to the database from JSON content.
	public IActionResult Add_Questions(JsonContent questions)
	{
		var materie_id = Convert.ToInt32(HttpContext.Request.Query["materie"]);
		ViewBag.materie = _db.Materii.Find(materie_id);

		return View(questions);
	}

	// Adds questions to the database from an uploaded PDF file.
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

        //Upload PDF to Flask API
        //http://46.101.136.24/

        var response = await UploadPdfToFlaskApiAsync(file, "http://46.101.136.24:5555/");

		if (response.IsSuccessStatusCode)
		{
			var responseString = await response.Content.ReadAsStringAsync();
			Console.WriteLine(responseString);

			var questions = await response.Content.ReadAsStringAsync();
			var questions1 = JsonConvert.DeserializeObject<QuestionDtoList>(questions);

			foreach (var questionDto in questions1.Questions)
			{
				var stringrasp = "";
				var intrebareRasp = new IntrebariRasp
				{
					intrebare = questionDto.Question,
					validareProfesor = 0,
					MaterieId = materie_id,
					Variante = questionDto.Choices.Select(choice => new Variante
					{
						Choice = choice.Value,
						VariantaCorecta = choice.Key == questionDto.Answer ? 1 : 0
                    }).ToList(),
					raspunsCorect = questionDto.Choices.FirstOrDefault(choice => choice.Key == questionDto.Answer).Value
                };

				_db.IntrebariRasps.Add(intrebareRasp);
			}

			_db.SaveChanges();
			ViewBag.materie = _db.Materii.Find(materie_id);
            TempData["SuccessMessage"] = "Added questions successfully";
            return RedirectToAction("Admin", "Home"); //Pass the Quiz Model to the View
		}
		else
		{
			//Handle API upload error
			return StatusCode((int)response.StatusCode, $"Error uploading file: {response.ReasonPhrase}");
		}
	}

	// Checks if the uploaded file is a PDF.
	private bool IsPdfFile(IFormFile file)
	{
		return file.ContentType.Contains("application/pdf");
	}

	// Uploads a PDF file to the Flask API and returns the response.
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
}
