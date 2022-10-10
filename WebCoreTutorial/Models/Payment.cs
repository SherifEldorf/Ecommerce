using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTutorial.Models
{
    public class Payment
    {
        public int id { get; set; }

        [Display(Name = "نوع البطاقة"), StringLength(70), Required(ErrorMessage = "نوع البطاقة مطلوب")]
        public string cardType { get; set; }

        [Display(Name = "اسم البطاقة"), StringLength(70), Required(ErrorMessage = "اسم البطاقة مطلوب")]
        public string cardName { get; set; }

        [Display(Name = "رقم البطاقة"), DataType(DataType.CreditCard), Required(ErrorMessage = "رقم البطاقة مطلوب")]
        public long cardNumber { get; set; }

        [Display(Name = "تاريخ الانتهاء"), DataType(DataType.DateTime), Required(ErrorMessage = "تاريخ انتهاء البطاقة مطلوب")]
        public DateTime expiration { get; set; }

        [Display(Name = "رمز التحقق من البطاقة"), Required(ErrorMessage = "رمز التحقق من البطاقة مطلوب")]
        public int cvv { get; set; }

        public int cartId { get; set; }
        [ForeignKey("cartId")]
        public Cart Cart { get; set; }

        public int billingId { get; set; }
        [ForeignKey("billingId")]
        public BillingAddress billingAddress { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser appUser { get; set; }
    }
}
