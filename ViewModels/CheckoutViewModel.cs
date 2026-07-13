using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.ViewModels
{
    public class CheckoutViewModel
    {
        [Display(Name = "عنوان الشحن")]
        [Required(ErrorMessage = "حقل عنوان الشحن مطلوب")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Display(Name = "المدينة")]
        [Required(ErrorMessage = "حقل المدينة مطلوب")]
        public string ShippingCity { get; set; } = string.Empty;

        [Display(Name = "الرمز البريدي")]
        [Required(ErrorMessage = "حقل الرمز البريدي مطلوب")]
        public string ShippingPostalCode { get; set; } = string.Empty;

        [Display(Name = "الدولة")]
        [Required(ErrorMessage = "حقل الدولة مطلوب")]
        public string ShippingCountry { get; set; } = string.Empty;

        [Display(Name = "طريقة الدفع")]
        [Required(ErrorMessage = "الرجاء اختيار طريقة الدفع")]
        public string PaymentMethod { get; set; } = string.Empty;

        // هذا الحقل لا يظهر للمستخدم لذلك لا يحتاج Display
        public CartViewModel? Cart { get; set; }
    }
}