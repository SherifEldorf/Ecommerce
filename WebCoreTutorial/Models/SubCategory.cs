using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTutorial.Models
{
    public class SubCategory
    {
        public int id { get; set; }

        [StringLength(100), Display(Name = "التصنيفات الفرعية"), Required(ErrorMessage = "اسم التصنيف الفرعي مطلوب")]
        public string SubCatName { get; set; }

        public int catId { get; set; }
        [ForeignKey("catId")]
        public Category Category { get; set; }
    }
}
