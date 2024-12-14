using Ecommerce.Entities.Settings;
using Ecommerce.Entities.ViewModels.AdminOrders;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orderHeaders = await _unitOfWork.OrderHeaders
                .GetAll(includes: new[] { "ApplicationUser" });
            return View(orderHeaders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            OrderVM model = new OrderVM
            {
                OrderHeader = await _unitOfWork.OrderHeaders
                .Find(u => u.Id == id, includes: new[] { "ApplicationUser" }),
                OrderDetails = await _unitOfWork.OrderDetails
                .GetAll(d => d.OrderId == id, includes: new[] { "Product" })
            };

            return View(model);
        }
    }
}
