using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTutorial.Models
{
    public class Cart
    {
        public int id { get; set; }

        [Display(Name = "اسم المنتج"), StringLength(70)]
        public string ProductName { get; set; }

        [Display(Name = "السعر"), DataType(DataType.Currency)]
        public double? Price { get; set; }

        [Display(Name = "الخصم")]
        public int? Discount { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser appUser { get; set; }
    }
}
