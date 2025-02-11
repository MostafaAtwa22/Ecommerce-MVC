using AutoMapper;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork unitOfWork, 
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Index()
        {
            var categories = await _unitOfWork.Categories.GetAll();
            return View(categories);
        }

        [HttpGet]
        public async Task<ActionResult<Category>> Details(int id)
        {
            var category = await _unitOfWork.Categories.Find(c => c.Id == id);

            if (category is null)
                return NotFound();

            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = _mapper.Map<Category>(model);

            _unitOfWork.Categories.Create(category);
            await _unitOfWork.Complete();

            TempData["Create"] = "Data has been Created Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _unitOfWork.Categories
                .Find(c => c.Id == id);

            var categoryVM = _mapper.Map<EditCategoryVM>(category);
            return View(categoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = _mapper.Map<Category>(model);

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.Complete();

            TempData["Edit"] = "Data has been Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Categories
                .FindWithTrack(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.Complete();

            TempData["Delete"] = "Data has been Deleted Succesfully";
            return Ok();
        }
    }
}
