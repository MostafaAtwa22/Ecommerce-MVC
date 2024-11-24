using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.DataAccess.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SelectListItem>> GetSelectList()
            => await _context.Categories
                .Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() })
                .OrderBy(d => d.Text)
                .ToListAsync();
    }
}
