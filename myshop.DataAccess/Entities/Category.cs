using myshop.DataAccess.Common;
using System.ComponentModel.DataAnnotations;

namespace myshop.DataAccess.Entities
{
    public class Category : BaseEntity
    {

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
