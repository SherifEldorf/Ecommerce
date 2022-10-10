using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTutorial.Models
{
    public class AppRole
    {
        [StringLength(450)]
        public string id { get; set; }

        [StringLength(150), Display(Name = "اسم الصلاحية")]
        public string RoleName { get; set; }
    }
}
