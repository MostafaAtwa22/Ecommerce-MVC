﻿using Ecommerce.Entities.Models;

namespace Ecommerce.Entities.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        void Update(Product model);
    }
}
