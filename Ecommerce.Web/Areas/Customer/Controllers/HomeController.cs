using Ecommerce.Entities.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<ShoppingCartVM>> Details(int? id)
        {
            var Product = await _unitOfWork.Products.Find(c => c.Id == id, new[] { "Category" });

            if (Product is null)
                return NotFound();

            ShoppingCartVM model = new ShoppingCartVM
            {
                Product = Product,
                Count = 1
            };
            return View(model); 
        }
    }
}
