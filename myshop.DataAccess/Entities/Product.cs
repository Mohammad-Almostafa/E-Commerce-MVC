using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using myshop.DataAccess.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.DataAccess.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [DisplayName("Image")]
        [ValidateNever]
        public string Img { get; set; }

        public decimal Price { get; set; }

        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
