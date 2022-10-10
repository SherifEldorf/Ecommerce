using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTutorial.Models
{
    public class BillingAddress
    {
        public int id { get; set; }

        [Display(Name = "الاسم الاول"), StringLength(50), Required(ErrorMessage = "الاسم الاول مطلوب")]
        public string firstName { get; set; }

        [Display(Name = "اسم العائلة"), StringLength(50), Required(ErrorMessage = "اسم العائلة مطلوب")]
        public string lastName { get; set; }

        [StringLength(250), Required(ErrorMessage = "اسم المستخدم مطلوب"), Display(Name = "اسم المستخدم")]
        public string UserName { get; set; }

        [StringLength(250), DataType(DataType.EmailAddress), Required(ErrorMessage = "البريد الالكتروني مطلوب"), Display(Name = "البريد الالكتروني")]
        public string Email { get; set; }

        [Display(Name = "العنوان"), StringLength(250), Required(ErrorMessage = "العنوان مطلوب")]
        public string Address { get; set; }

        [Display(Name = "مكان الاقامة"), StringLength(50), Required(ErrorMessage = "مكان الاقامة مطلوب")]
        public string Country { get; set; }

        [Display(Name = "بريد البلد"), StringLength(50), Required(ErrorMessage = "بريد البلد مطلوب")]
        public int Zip { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser appUser { get; set; }
    }
}
