using Ecommerce.Entities.ViewModels.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ActionResult<IEnumerable<Product>>> Index()
        {
            var products = await _unitOfWork.Products.GetAll();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _unitOfWork.Products.Find(c => c.Id == id, new[] { "Category" });

            if (product is null)
                return NotFound();

            ShoppingCart model = new ShoppingCart
            {
                Product = product,
                Count = 1
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart model)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            model.ApplicationUserId = claim?.Value!;

            ShoppingCart? cart = await _unitOfWork.ShoppingCarts
                .FindWithTrack(c => c.ApplicationUserId == claim.Value! && c.ProductId == model.ProductId);

            if (cart is null)
            {
                model.Id = 0;
                // Add to database
                _unitOfWork.ShoppingCarts.Create(model);
            }
            else
            {
                _unitOfWork.ShoppingCarts.IncreaseCount(cart, model.Count);
            }
            await _unitOfWork.Complete();

            return RedirectToAction("Index");
        }
    }
}       
