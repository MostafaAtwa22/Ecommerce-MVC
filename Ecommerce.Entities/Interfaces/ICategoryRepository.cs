using Ecommerce.Entities.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Entities.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<SelectListItem>> GetSelectList();
    }
}
