﻿namespace Ecommerce.DataAccess.Repositories
{
    public class ApplicationUserRepository : GenericRepository<ApplicationUser> ,IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
