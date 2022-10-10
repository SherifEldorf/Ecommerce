using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTutorial.Models
{
    public class AppConfirm
    {
        [StringLength(450)]
        public string id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser appUser { get; set; }

        public DateTime DateConfirm { get; set; }
    }
}
