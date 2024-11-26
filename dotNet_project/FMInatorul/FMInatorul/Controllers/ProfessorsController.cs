﻿using FMInatorul.Data;
using FMInatorul.Migrations;
using FMInatorul.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FMInatorul.Controllers
{
	public class ProfessorsController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext db;

		public ProfessorsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			db = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		// Returns the index view for professors.
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Challenge();
			}
			var profesor = await db.Professors.FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);
			if (profesor.CompletedProfile == false)
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
			return View();
		}

        // Returns the view to edit the subject for a professor.
        public async Task<IActionResult> EditMaterie()
        {
            var userId = _userManager.GetUserId(User);
            var profesor = await db.Professors
                .Include(p => p.MateriiPredate)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

            if (profesor == null)
            {
                return NotFound();
            }

            // Obține toate materiile din facultatea profesorului
            var materii = await db.Materii
                .Where(m => m.FacultateID == profesor.FacultateID)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.nume
                })
                .ToListAsync();

            profesor.Materii = materii;

            // Setează materiile selectate
            profesor.SelectedMateriiIds = profesor.MateriiPredate?.Select(m => m.Id).ToList();

            return View(profesor);
        }


        // Handles the post request to edit the subject for a professor.
        [HttpPost]
        public async Task<IActionResult> EditMaterie(int id, Profesor profesor)
        {
            if (ModelState.IsValid) 
            {
                TempData["ErrorMessage"] = "Please select at least one subject.";
                return RedirectToAction("EditMaterie", new { id });
            }

            var profesorToUpdate = await db.Professors
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profesorToUpdate == null)
            {
                return NotFound();
            }

            // Actualizează materiile predate
            profesorToUpdate.MateriiPredate = (ICollection<Materie>?)profesor.SelectedMateriiIds;

            profesorToUpdate.CompletedProfile = true;

            await db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Materii updated successfully.";
            return RedirectToAction("Index", "Professors");
        }


        // Returns the view to validate or delete questions for a professor.
        public async Task<IActionResult> Intrebari()
		{
			var user = await _userManager.GetUserAsync(User);
            var profesor = await db.Professors.Include(p => p.MateriiPredate).FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (profesor == null || profesor.MateriiPredate == null || !profesor.MateriiPredate.Any())
			{
				TempData["ErrorMessage"] = "Please finish completing your profile.";
				return RedirectToAction("Index", "Professors");
			}

            var materiiPredateIds = profesor.MateriiPredate.Select(m => m.Id).ToList();

            var intrebari = await db.IntrebariRasps
                .Where(i => materiiPredateIds.Contains(i.MaterieId) && i.validareProfesor == 0)
                .ToListAsync();

            if (TempData.ContainsKey("SuccessMessage"))
			{
				ViewBag.Message = TempData["SuccessMessage"];
				return View(intrebari);
			}

			return View(intrebari);
		}

        public async Task<IActionResult> EditCollegeProf()
        {
            var userId = _userManager.GetUserId(User);
            var profesor = await db.Professors.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

            if (profesor == null)
            {
                TempData["ErrorMessage"] = "Profesor not found.";
                return RedirectToAction("Index");
            }

            // Populate the ViewBag with faculties for the dropdown list
            var facultati = await db.Facultati.Where(f => f.Id != 1).ToListAsync();

            ViewBag.Facultati = facultati.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.nume
            }).ToList();


            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.Message = TempData["ErrorMessage"];
                return View(profesor);
            }

            return View(profesor);
        }

        // Handles the post request to edit the student's semester.
        [HttpPost]
        public IActionResult EditCollegeProf(int id, Profesor profesor)
        {
            if (ModelState.IsValid)
            {
                return View(profesor);
            }
            else
            {
                Profesor profesorToUpdate = db.Professors.Where(stu => stu.Id == id).FirstOrDefault();

                if (profesorToUpdate == null)
                {
                    return NotFound();
                }

                profesorToUpdate.FacultateID = profesor.FacultateID;

                if (profesor.FacultateID == 1 || profesor.FacultateID == null)
                {
                    TempData["ErrorMessage"] = "A college must be selected";
                    return RedirectToAction("EditCollegeProf", "Professors");
                }
                db.SaveChanges();
                TempData["SuccessMessage"] = "College updated successfully.";
                return RedirectToAction("Index", "Professors");
            }
        }


        // Validates a question by setting its validation status.
        public async Task<IActionResult> Valideaza(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var intrebare = await db.IntrebariRasps.FindAsync(id);
			if (intrebare == null)
			{
				return NotFound();
			}

			intrebare.validareProfesor = 1;
			db.SaveChanges();

			TempData["SuccessMessage"] = "Intrebarea a fost validata cu succes";
			return RedirectToAction("Intrebari", "Professors");
		}

		// Deletes a question.
		public async Task<IActionResult> NuValideaza(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var intrebare = await db.IntrebariRasps.FindAsync(id);
			if (intrebare == null)
			{
				return NotFound();
			}

			db.IntrebariRasps.Remove(intrebare);
			db.SaveChanges();

			TempData["SuccessMessage"] = "Intrebarea a fost stearsa cu succes";
			return RedirectToAction("Intrebari", "Professors");
		}
	}
}
