using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTutorial.Models
{
    public class UserSetting
    {
        public int id { get; set; }

        [Display(Name = "ارسال بريد للتحقق من التسجيل؟")]
        public bool isEmailConfirm { get; set; }

        [Display(Name = "هل التسجيل مفتوح؟")]
        public bool isRegisterOpen { get; set; }

        [Display(Name = "أقل طول مقطع الباسوورد")]
        public int MinimumPassLength { get; set; }

        [Display(Name = "اطول مقطع للباسوورد")]
        public int MaxPassLength { get; set; }

        [Display(Name = "هل الباسوورد يحتوي علي الاقل رقم واحد؟")]
        public bool isDigit { get; set; }

        [Display(Name = "هل يحنوي الباسوورد علي حرف كابيتل بالانجليزي؟")]
        public bool isUpper { get; set; }

        [Display(Name = "ارسال رسالة ترحيب بعد نجاح التسجيل؟")]
        public bool SendWelcomeMessage { get; set; }
    }
}
