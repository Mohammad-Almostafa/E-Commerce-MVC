using myshop.Entities.Models;

namespace myshop.DataAccess.Rebositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // نقوم هنا بتعريف الـ Repositories التي يضمها المشرف العام
        IProduct Products { get; }
        ICategory Categories { get; }
        IOrderDetail OrderDetails { get; }
        IOrderHeader OrderHeaders { get; }
        IReview Reviews { get; }

        // الدالة الموحدة لحفظ جميع التغييرات في خطوة واحدة بالـ Database
        Task<int> CompleteAsync();
    }
}