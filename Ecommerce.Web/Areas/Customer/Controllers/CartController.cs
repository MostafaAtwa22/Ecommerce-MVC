using Ecommerce.Entities.ViewModels.Customer;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Stripe.Checkout;
using System.Security.Claims;

namespace Ecommerce.Web.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]
    public class CartController : Controller
    {
		private readonly IUnitOfWork _unitOfWork;
		public ShoppingCartVM ShoppingCartVM { get; set; }

		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM
            {
                CartList = await _unitOfWork.ShoppingCarts
                .GetAll(u => u.ApplicationUserId == claim!.Value, 
                includes: new[] { "Product" }),
                OrderHeader = new()
            };

            foreach (var item in ShoppingCartVM.CartList)
                ShoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
            return View(ShoppingCartVM);
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM
            {
                CartList = await _unitOfWork.ShoppingCarts
                .GetAll(u => u.ApplicationUserId == claim!.Value,
                includes: new[] { "Product" }),
                OrderHeader = new ()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = await _unitOfWork.ApplicationUsers
                .FindWithTrack(u => u.Id == claim!.Value);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.Phone = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (var item in ShoppingCartVM.CartList)
                ShoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Summary(ShoppingCartVM model)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            model.CartList = await _unitOfWork.ShoppingCarts
                .GetAll(u => u.ApplicationUserId == claim!.Value,
                includes: new[] { "Product" });

            model.OrderHeader.OrderStatus = SD.Pending;
            model.OrderHeader.PaymentStatus = SD.Pending;
            model.OrderHeader.OrderDate = DateTime.Now;
            model.OrderHeader.ApplicationUserId = claim!.Value;

            foreach (var item in model.CartList)
                model.OrderHeader.TotalPrice += (item.Count * item.Product.Price);

            _unitOfWork.OrderHeaders.Create(model.OrderHeader);
            await _unitOfWork.Complete();

            foreach (var item in model.CartList)
            {
                OrderDetails orderDetails = new OrderDetails
                {
                    ProductId = item.ProductId,
                    Price = item.Product.Price,
                    Quantity = item.Count,
                    OrderId = model.OrderHeader.Id
                };

                _unitOfWork.OrderDetails.Create(orderDetails);
                await _unitOfWork.Complete();
            }

            var domain = "https://localhost:7220/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={model.OrderHeader.Id}",
                CancelUrl = domain + "Customer/Cart/Index"
            };

            foreach (var item in model.CartList)
            {
                var sessionLineOption = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        }
                    },
                    Quantity = item.Count
                };

                options.LineItems.Add(sessionLineOption);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            model.OrderHeader.SessionID = session.Id;
            model.OrderHeader.PaymentIntentId = session.PaymentIntentId;
            await _unitOfWork.Complete();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            OrderHeader? orderHeader = await _unitOfWork.OrderHeaders
                .FindWithTrack(u => u.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader!.SessionID);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeaders.UpdateOrderStatus(id, SD.Approve, SD.Approve);
                orderHeader.PaymentIntentId = session.PaymentIntentId;
                await _unitOfWork.Complete();
            }
            List<ShoppingCart> shoppingCarts = (List<ShoppingCart>)await _unitOfWork.ShoppingCarts
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId);
            //HttpContext.Session.Clear();
            _unitOfWork.ShoppingCarts.RemoveRange(shoppingCarts);
            await _unitOfWork.Complete();

            return View(id);
        }

        [HttpGet]
        public async Task<IActionResult> Plus(int id)
        {
            var cart = await _unitOfWork.ShoppingCarts
                .FindWithTrack(u => u.Id == id);

            if (cart is null)
                return NotFound();

            _unitOfWork.ShoppingCarts.IncreaseCount(cart, 1);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Minus(int id)
        {
            var cart = await _unitOfWork.ShoppingCarts
             .FindWithTrack(u => u.Id == id);

            if (cart is null)
                return NotFound();

            if (cart.Count <= 1)
            { 
                _unitOfWork.ShoppingCarts.Delete(cart);
                await _unitOfWork.Complete();
                return RedirectToAction("Index", "Home");
            }
            _unitOfWork.ShoppingCarts.DecreaseCount(cart, 1);
            await _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var cart = await _unitOfWork.ShoppingCarts
                .FindWithTrack(u => u.Id == id);

            if (cart == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            _unitOfWork.ShoppingCarts.Delete(cart);
            await _unitOfWork.Complete();

            return Json(new { success = true, message = "User deleted successfully." });
        }
    }
}
