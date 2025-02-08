using Ecommerce.Entities.ViewModels;
using Ecommerce.Models.ViewModels;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using X.PagedList.Extensions;

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
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 2; 

            var products = await _unitOfWork.Products.GetAll(includes: new[] { "Category" });

            var productsByCategory = products
                .GroupBy(p => new { p.CategoryId, p.Category.Name })
                .ToDictionary(
                    g => new EditCategoryVM { Id = g.Key.CategoryId, Name = g.Key.Name },
                    g => g.ToList()
                );

            var categoriesList = productsByCategory.ToList();

            var paginatedCategories = categoriesList.ToPagedList(pageNumber, pageSize);

            var latestProducts = products
                .OrderByDescending(p => p.TimeCreation)
                .Take(5)
                .ToList();

            var model = new HomeViewModel
            {
                PaginatedCategories = paginatedCategories,
                BannerProducts = latestProducts
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _unitOfWork.Products.Find(c => c.Id == id, new[] { "Category" });

            if (product is null)
                return NotFound();

            ShoppingCart cart = new ShoppingCart
            {
                Product = product,
                Count = 1
            };

            var ProductsByCategory = await _unitOfWork.Products
                .GetAll(p => p.CategoryId == cart.Product.CategoryId && p.Id != id);

            ViewBag.ProductsByCategory = ProductsByCategory.ToList();

            return View(cart);
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
                await _unitOfWork.Complete();
                var UserCart = await _unitOfWork.ShoppingCarts.GetAll(x => x.ApplicationUserId == claim.Value);
                HttpContext.Session.SetInt32(SD.SessionKey,  UserCart.ToList().Count());
            }
            else
            {
                _unitOfWork.ShoppingCarts.IncreaseCount(cart, model.Count);
                await _unitOfWork.Complete();
            }

            return RedirectToAction("Index");
        }
    }
}




