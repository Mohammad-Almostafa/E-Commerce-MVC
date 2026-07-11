using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Entities;
using myshop.DataAccess.Rebositories.Interfaces;
using myshop.DataAccess.Repositories;
using myshop.Entities.Models;

namespace myshop.DataAccess.Rebositories
{
    public class ProductRepository : GenericRepository<Product>, IProduct
    {
        private readonly ApplicationDbContext context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
        {
            return await context.Products.Include(x => x.Category).ToListAsync();
        }
    }
}
