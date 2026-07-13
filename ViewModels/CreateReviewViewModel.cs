using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.ViewModels
{
    public class CreateReviewViewModel
    {
        [Required(ErrorMessage = "حقل معرف الكتاب مطلوب")]
        public int BookId { get; set; }

        [Display(Name = "التقييم")]
        [Required(ErrorMessage = "الرجاء اختيار التقييم")]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5")]
        public int Rating { get; set; }

        [Display(Name = "التعليق")]
        [StringLength(1000, ErrorMessage = "التعليق لا يمكن أن يتجاوز 1000 حرف")]
        public string? Comment { get; set; }
    }
}