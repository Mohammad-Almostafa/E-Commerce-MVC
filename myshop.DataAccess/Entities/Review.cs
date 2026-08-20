using myshop.DataAccess.Common;
using System.ComponentModel.DataAnnotations;

namespace myshop.DataAccess.Entities
{
    public class Review : BaseEntity
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required, MaxLength(1000)]
        public string Comment { get; set; } = null!;

        // Foreign Keys
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}