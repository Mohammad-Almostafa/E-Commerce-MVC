using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using myshop.DataAccess.Common;
using myshop.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace myshop.DataAccess.Entities.Models
{
    public class ShoppingCart : BaseEntity
    {
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }

        public int Count { get; set; }

        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
