using FMInatorul.Data;
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

        public ProfessorsController(ApplicationDbContext context,
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
            var profesor = await db.Professors
                                    .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);
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




        public async Task<IActionResult> EditMaterie()
        {
            var selectList = new List<SelectListItem>();
            var materii = from m in db.Materii select m;
            foreach (var item in materii)
            {
                selectList.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.nume.ToString()
                });

            }

            
            var userId = _userManager.GetUserId(User);
            var profesor = await db.Professors
                                        .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            profesor.Materii = selectList;
            if (profesor == null)
            {
                return NotFound();
            }
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.Message = TempData["ErrorMessage"];
                return View(profesor);
            }
            return View(profesor);
        }

        [HttpPost]
        public IActionResult EditMaterie(int id, Profesor prof)
        {
            if (ModelState.IsValid)
            {
                return View(prof);
            }
            else
            {

                Profesor profToUpdate = db.Professors.Where(pr => pr.Id == id).First();

                if (profToUpdate == null)
                {
                    return NotFound();
                }

                if(prof.MaterieId == null)
                {
                    TempData["ErrorMessage"] = "Please select a subject.";
                    return RedirectToAction("EditMaterie", "Professors");
                }
                profToUpdate.MaterieId = prof.MaterieId;
                profToUpdate.CompletedProfile = true;

                db.SaveChanges();
                TempData["SuccessMessage"] = "Materie updated successfully.";
                return RedirectToAction("Index", "Professors");
            }

        }




        public async Task<IActionResult> Intrebari()
        {
            var user = await _userManager.GetUserAsync(User);
            var profesor = await db.Professors
                                    .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (profesor == null || profesor.MaterieId == null)
            {
                TempData["ErrorMessage"] = "Please finish completing your profile.";
                return RedirectToAction("Index", "Professors");
            }

            var intrebari = await db.IntrebariRasps
                .Where(i => i.MaterieId == profesor.MaterieId && i.validareProfesor == 0)
                .ToListAsync();

            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.Message = TempData["SuccessMessage"];
                return View(intrebari);
            }

            return View(intrebari);
        }

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
