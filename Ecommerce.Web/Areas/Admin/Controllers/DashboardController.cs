using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _unitOfWork.OrderHeaders.GetAll();
            ViewBag.Orders = orders.Count();
            var categories = await _unitOfWork.Categories.GetAll();
            ViewBag.Categories = categories.Count();
            var users = await _unitOfWork.ApplicationUsers.GetAll();
            ViewBag.Users = users.Count();
            var products = await _unitOfWork.Products.GetAll();
            ViewBag.Products = products.Count();

            return View();
        }
    }
}
