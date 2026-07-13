using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.ViewModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }

        [Display(Name = "عنوان الكتاب")]
        [Required(ErrorMessage = "حقل عنوان الكتاب مطلوب")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "المؤلف")]
        [Required(ErrorMessage = "حقل المؤلف مطلوب")]
        [StringLength(150)]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "الترقيم الدولي (ISBN)")]
        [Required(ErrorMessage = "حقل الترقيم الدولي مطلوب")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "يجب أن يكون الرقم بين 10 إلى 13 رقم")]
        public string ISBN { get; set; } = string.Empty;

        [Display(Name = "السعر")]
        [Required(ErrorMessage = "حقل السعر مطلوب")]
        [Range(0.01, 10000, ErrorMessage = "السعر يجب أن يكون بين 0.01 و 10000")]
        public decimal Price { get; set; }

        [Display(Name = "الوصف")]
        [StringLength(2000)]
        public string? Description { get; set; }

        [Display(Name = "رابط الصورة")]
        public string? ImageUrl { get; set; }

        [Display(Name = "الكمية المتوفرة")]
        [Range(0, 10000, ErrorMessage = "الكمية يجب أن تكون بين 0 و 10000")]
        public int StockQuantity { get; set; }

        [Display(Name = "التصنيف")]
        [Required(ErrorMessage = "الرجاء اختيار التصنيف")]
        public int CategoryId { get; set; }

        [Display(Name = "اسم التصنيف")]
        public string? CategoryName { get; set; }

        [Display(Name = "التقييم")]
        public int? Rating { get; set; } // علامة الاستفهام تجعله اختيارياً ولا يسبب خطأ إذا كان فارغاً
    }
}