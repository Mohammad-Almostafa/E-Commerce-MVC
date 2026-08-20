using System.ComponentModel.DataAnnotations;
using myshop.Business.DTOs;

namespace myshop.Web.ViewModels
{
    public class CheckoutVM
    {
        public Cart Cart { get; set; } = new Cart();

        [Required(ErrorMessage = "pleas enter name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "pleas enter phone number")]
        [Phone(ErrorMessage = "phone number is incorrect")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "pleas enter address")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "pleas enter city")]
        public string City { get; set; } = null!;
    }
}