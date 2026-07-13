using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "الاسم الكامل")]
        [Required(ErrorMessage = "حقل الاسم الكامل مطلوب")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "البريد الإلكتروني")]
        [Required(ErrorMessage = "حقل البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "كلمة المرور")]
        [Required(ErrorMessage = "حقل كلمة المرور مطلوب")]
        [StringLength(100, ErrorMessage = "يجب أن تكون كلمة المرور {2} أحرف على الأقل.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "تأكيد كلمة المرور")]
        [Required(ErrorMessage = "حقل تأكيد كلمة المرور مطلوب")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقتين.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}