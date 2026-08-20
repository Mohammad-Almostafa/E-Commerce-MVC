using myshop.DataAccess.Rebositories;
using myshop.DataAccess.Rebositories.Interfaces;

namespace myshop.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        
        // قراءة فقط (Read-only properties) للـ Repositories
        public IProduct Products { get; private set; }
        
        public ICategory Categories { get; private set; }

        public IOrderDetail OrderDetails { get; private set; }

        public IOrderHeader OrderHeaders { get; private set; }

        public IReview Reviews { get; private set; }

        // نقوم بحقن الـ ApplicationDbContext ليتشاركه جميع الـ Repositories
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            // تهيئة الـ Repositories وتمرير الـ Context المشترك لها
            Products = new ProductRepository(_context);
            Categories = new CategoryRepository(_context);
            OrderDetails = new OrderDetailRepository(_context);
            OrderHeaders = new OrderHeaderRepository(_context);
            Reviews = new ReviewRepository(_context);
        }


        // استدعاء الحفظ الفعلي لـ Entity Framework مرة واحدة
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // تفريغ الـ Context من الذاكرة بأمان عند انتهاء الطلب (Request)
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}