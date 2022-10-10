using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebCoreTutorial.Models
{
    public class AppUser
    {
        [StringLength(450)]
        public string id { get; set; }

        [StringLength(250), Required(ErrorMessage = "اسم المستخدم مطلوب"), Display(Name = "اسم المستخدم")]
        public string UserName { get; set; }

        [StringLength(250), Required(ErrorMessage = "البريد الالكتروني مطلوب"), Display(Name = "البريد الالكتروني"), DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(650), Required(ErrorMessage = "كلمة المرور مطلوبة"), Display(Name = "كلمة المرور"), DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "كلمتا المرور غير متطابقتين") ,StringLength(650), Required(ErrorMessage = "اعادة كتابة كلمة المرور مطلوبة"), Display(Name = "اعادة كتابة كلمة المرور"), DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        [StringLength(250), Display(Name = "الهاتف"), DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "تاكيد البريد الالكتروني")]
        public bool EmailConfirm { get; set; }

        [ScaffoldColumn(true)]
        public bool Lockout { get; set; }

        [ScaffoldColumn(true)]
        public DateTime? LockTime { get; set; }

        [ScaffoldColumn(true)]
        public int ErrorLogCount { get; set; }
    }
}
