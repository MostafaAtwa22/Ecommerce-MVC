using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var claminIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claminIdentity.FindFirst(ClaimTypes.NameIdentifier);

            string? userId = claim?.Value;


            return View(_context.ApplicationUsers
                .Where(u => u.Id != userId)
                .ToList());
        }

        public IActionResult LockUnLock(string? id)
        {
            var user = _context.ApplicationUsers
                .FirstOrDefault(u => u.Id == id);

            if (user is null)
                return NotFound();

            if (user.LockoutEnd is null || user.LockoutEnd < DateTime.Now)
                user.LockoutEnd = DateTime.Now.AddMonths(1);
            else
                user.LockoutEnd = DateTime.Now;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            _context.ApplicationUsers.Remove(user);
            _context.SaveChanges();

            return Json(new { success = true, message = "User deleted successfully." });
        }
    }
}
