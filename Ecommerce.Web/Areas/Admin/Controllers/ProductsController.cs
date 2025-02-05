using Ecommerce.Entities.Settings;
using Ecommerce.Entities.ViewModels.Products;
using Stripe.V2;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _imagesPath;

        public ProductsController(IUnitOfWork unitOfWork, 
            IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _imagesPath = $"{_webHostEnvironment.WebRootPath}{FileSettings.ImagesPath}/Products";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Index()
        {
            var products = await _unitOfWork.Products.GetAll(includes: new[] { "Category" });
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _unitOfWork.Products
                .GetAll(includes: new[] { "Category" });

            var model = products.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category?.Name,
                Price = p.Price,
                Image = p.Image,
            }).ToList();

            return Json(new { data = model });
        }

        [HttpGet]
        public async Task<ActionResult<Product>> Details(int id)
        {
            var Product = await _unitOfWork.Products.Find(c => c.Id == id, new[] { "Category" });

            if (Product is null)
                return NotFound();

            return View(Product);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new()
            {
                CategoriesList = await _unitOfWork.Categories.GetSelectList()
            };
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductVM model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoriesList = await _unitOfWork.Categories.GetSelectList();
                return View(model);
            }

            var imageName = await SaveImage(model.Image);

            Product product = new Product
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                Image = imageName,
                Description = model.Description,
                Price = model.Price,
                Amount = model.Quantity
            };

            _unitOfWork.Products.Create(product);
            var effectedRows = await _unitOfWork.Complete();
            if (effectedRows > 0)
                TempData["Create"] = "Data has been Created Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Products
                .FindWithTrack(c => c.Id == id);

            if (product is null)
                return NotFound();

            EditProductVM productVM = new EditProductVM
            {
                Id = product.Id,
                Name = product.Name,
                CategoryId = product.CategoryId,
                Description = product.Description,
                Price = product.Price,
                CategoriesList = await _unitOfWork.Categories.GetSelectList(),
                CurrentImage = product.Image,
                Quantity = product.Amount
            };
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductVM model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoriesList = await _unitOfWork.Categories.GetSelectList();
                return View(model);
            }

            var product = await _unitOfWork.Products.FindWithTrack(p => p.Id ==  model.Id);

            if (product is null)
                return NotFound();

            var hasNewImage = model.Image is not null;
            var oldImage = product.Image;

            product.Id = model.Id;
            product.Name = model.Name;
            product.CategoryId = model.CategoryId;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Amount = model.Quantity;


            if (hasNewImage)
                product.Image = await SaveImage(model.Image!);

            _unitOfWork.Products.Update(product);
            var effectedRows = await _unitOfWork.Complete();

            if (effectedRows > 0)
            {
                if (hasNewImage)
                {
                    var cover = Path.Combine(_imagesPath, oldImage);
                    if (System.IO.File.Exists(cover))
                        System.IO.File.Delete(cover);
                }
                TempData["Edit"] = "Data has been Updated Successfully";
                return RedirectToAction(nameof(Index));
            }

            else
            {
                var cover = Path.Combine(_imagesPath, product.Image);
                System.IO.File.Delete(cover);
                return BadRequest();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var Product = await _unitOfWork.Products
                .FindWithTrack(c => c.Id == id);

            if (Product == null)
                return Json(new { success = false, message = "Error Can't Deleted This item" });


            _unitOfWork.Products.Delete(Product);
            var effectedRows = await _unitOfWork.Complete();

            if (effectedRows > 0)
            {
                var image = Path.Combine(_imagesPath, Product.Image);
                if (System.IO.File.Exists(image))
                {
                    System.IO.File.Delete(image);
                }
            }

            return Json(new {success = true, message = "Data Deleted Successfully"});
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Invalid image file.");

            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var path = Path.Combine(_imagesPath, imageName);

            await using var stream = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(stream);

            return imageName;
        }
    }
}
