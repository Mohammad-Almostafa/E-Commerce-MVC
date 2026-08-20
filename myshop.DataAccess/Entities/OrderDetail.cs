using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using myshop.DataAccess.Common;

namespace myshop.DataAccess.Entities.Models
{
    public class OrderDetail : BaseEntity
    {
        public int OrderHeaderId { get; set; }
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }

        public int ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }

        public decimal Price { get; set; }

        public int Count { get; set; }
    }
}
