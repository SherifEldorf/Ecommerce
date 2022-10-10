using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebCoreTutorial.Models
{
    public class UserProfile
    {
        [Display(Name = "معرف الملف الشخصي")]
        public long id { get; set; }

        [Display(Name = "مكان الاقامة"), StringLength(100)]
        public string Country { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser appUser { get; set; }

        [Display(Name = "تاريح الميلاد"), DataType(DataType.Date)]
        public DateTime? DateOfBurth { get; set; }

        [Display(Name = "رابط موقعك"), StringLength(200), DataType(DataType.Url)]
        public string PersonalWebUrl { get; set; }

        [Display(Name = "الصورة"), StringLength(400)]
        public string UrlImage { get; set; }
    }
}
