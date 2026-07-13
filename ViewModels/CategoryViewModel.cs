using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        [Display(Name = "اسم التصنيف")]
        [Required(ErrorMessage = "حقل اسم التصنيف مطلوب")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "الوصف")]
        [StringLength(500)]
        public string? Description { get; set; }

        public int BookCount { get; set; }
    }
}