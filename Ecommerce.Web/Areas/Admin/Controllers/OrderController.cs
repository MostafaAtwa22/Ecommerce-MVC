using Ecommerce.Entities.Settings;
using Ecommerce.Entities.ViewModels.AdminOrders;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Stripe;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderViewModel { get; set; }
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateOrderDetails()
		{
            var order = await _unitOfWork.OrderHeaders.Find(u => u.Id == OrderViewModel.OrderHeader.Id);

            order.Name = OrderViewModel.OrderHeader.Name; 
            order.Phone = OrderViewModel.OrderHeader.Phone; 
            order.Address = OrderViewModel.OrderHeader.Address; 
            order.City = OrderViewModel.OrderHeader.City;

            if (OrderViewModel.OrderHeader.Carrier is not null)
                order.Carrier = OrderViewModel.OrderHeader.Carrier;

            if (OrderViewModel.OrderHeader.TrakcingNumber is not null)
                order.TrakcingNumber = OrderViewModel.OrderHeader.TrakcingNumber;

            _unitOfWork.OrderHeaders.Update(order);
            await _unitOfWork.Complete();

			TempData["Edit"] = "Data has been Updated Succesfully";
			return RedirectToAction(nameof(Details), new { id = order.Id });
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProccess()
        {
            _unitOfWork.OrderHeaders.UpdateOrderStatus(OrderViewModel.OrderHeader.Id,
                SD.Proccessing, null!);
            await _unitOfWork.Complete();

            TempData["Edit"] = "Order Status has been Updated Succesfully";
            return RedirectToAction(nameof(Details), new { id = OrderViewModel.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartShip()
        {
            var order = await _unitOfWork.OrderHeaders.Find(u => u.Id == OrderViewModel.OrderHeader.Id);
            order.TrakcingNumber = OrderViewModel.OrderHeader.TrakcingNumber;
            order.Carrier = OrderViewModel.OrderHeader.Carrier;
            order.OrderStatus = SD.Shipped;
            order.ShippingDate = DateTime.Now;

            _unitOfWork.OrderHeaders.Update(order);
            await _unitOfWork.Complete();

            TempData["Edit"] = "Order has been Shipped Succesfully";
            return RedirectToAction(nameof(Details), new { id = OrderViewModel.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder()
        {
            var order = await _unitOfWork.OrderHeaders.Find(u => u.Id == OrderViewModel.OrderHeader.Id);

            if (order.PaymentStatus is SD.Approve)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = order.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(option);

                _unitOfWork.OrderHeaders.UpdateOrderStatus(order.Id, SD.Cancelled, SD.Refund);
            }
            else
                _unitOfWork.OrderHeaders.UpdateOrderStatus(order.Id, SD.Cancelled, SD.Cancelled);

            await _unitOfWork.Complete();

            TempData["Edit"] = "Order has been Canceled Succesfully";
            return RedirectToAction(nameof(Details), new { id = OrderViewModel.OrderHeader.Id });
        }
    }
}
