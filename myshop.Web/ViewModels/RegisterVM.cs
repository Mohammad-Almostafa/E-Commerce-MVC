using System.ComponentModel.DataAnnotations;

namespace myshop.Web.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "FullName required")]
        [Display(Name = "FullName")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "The email format is incorrect.")]
        [Display(Name = "e_mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters.", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Corfirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adress required")]
        [Display(Name = "Adress")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City required")]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;
    }
}