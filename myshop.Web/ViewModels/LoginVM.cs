using System.ComponentModel.DataAnnotations;

namespace myshop.Web.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "The email format is incorrect.")]
        [Display(Name = "e-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}
