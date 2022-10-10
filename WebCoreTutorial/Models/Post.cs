using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTutorial.Models
{
    public class Post
    {
        public long id { get; set; }

        [StringLength(100), Display(Name = "عنوان الموضوع"), Required(ErrorMessage = "عنوان الموضوع مطلوب")]
        public string Title { get; set; }

        [StringLength(4000), Display(Name = "الموضوع"), Required(ErrorMessage = "الموضوع مطلوب"), DataType(DataType.MultilineText)]
        public string PostContent { get; set; }

        [StringLength(1000), Display(Name = "الصورة"), DataType(DataType.Upload)]
        public string PostImg { get; set; }

        [StringLength(250), Display(Name = "كاتب الموضوع"), Required(ErrorMessage = "اسم كاتب الموضوع مطلوب")]
        public string Auther { get; set; }

        [Display(Name = "تاريخ الموضوع")]
        public DateTime PostDate { get; set; }

        [Display(Name = "المشاهدات")]
        public int PostViews { get; set; }

        [ScaffoldColumn(true), Display(Name = "اعجبني")]
        public int PostLike { get; set; }

        [Display(Name = "اسم المعجب")]
        public string LikeUserName { get; set; }

        [Display(Name = "التصنيف الفرعي"), Required(ErrorMessage = "اسم التصنيف الفرعي مطلوب")]
        public int SubId { get; set; }

        [ForeignKey("SubId")]
        public SubCategory SubCategory { get; set; }

        [Display(Name = "نشر")]
        public bool IsPublish { get; set; }

        [Display(Name = "اسم المنتج"), StringLength(70)]
        public string ProductName { get; set; }

        [Display(Name = "السعر")]
        public double? Price { get; set; }

        [Display(Name = "الخصم")]
        public int? Discount { get; set; }
    }
}
