using myshop.DataAccess.Entities;
using myshop.DataAccess.Rebositories.Interfaces;
using myshop.DataAccess.Repositories;

namespace myshop.DataAccess.Rebositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategory
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        
    }
}
