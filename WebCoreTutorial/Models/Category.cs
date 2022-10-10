using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTutorial.Models
{
    public class Category
    {
        public int id { get; set; }

        [StringLength(50), Display(Name = "التصنيفات الأساسية"), Required(ErrorMessage = "اسم التصنيف الاساسي مطلوب")]
        public string catName { get; set; }
    }
}
