using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "البريد الإلكتروني")]
        [Required(ErrorMessage = "حقل البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "كلمة المرور")]
        [Required(ErrorMessage = "حقل كلمة المرور مطلوب")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "تذكرني")]
        public bool RememberMe { get; set; }
    }
}