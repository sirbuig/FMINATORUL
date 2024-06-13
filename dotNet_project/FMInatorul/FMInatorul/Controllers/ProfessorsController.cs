using FMInatorul.Data;
using FMInatorul.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            return View();
        }




        public async Task<IActionResult> editMaterie()
        {
            var Materii = db.Materii;
            ViewBag.materii = Materii;

            var userId = _userManager.GetUserId(User);
            var profesor = await db.Professors
                                        .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
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
        public IActionResult editMaterie(int id, Profesor prof)
        {
            if (ModelState.IsValid)
            {
                return View(prof);
            }
            else
            {

                Profesor profToUpdate = db.Professors.Where(stu => stu.Id == id).First();

                if (profToUpdate == null)
                {
                    return NotFound();
                }

                profToUpdate.MaterieId = prof.MaterieId;
               
              
                db.SaveChanges();
                TempData["SuccessMessage"] = "Materie updated successfully.";
                return RedirectToAction("Index", "Professors");
            }

        }






    }
}
