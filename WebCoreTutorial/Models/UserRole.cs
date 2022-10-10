using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebCoreTutorial.Models
{
    public class UserRole
    {
        [StringLength(450)]
        public string id { get; set; }

        public string RoleId { get; set; }
        [ForeignKey("RoleId")]
        public AppRole  appRole { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser appUser { get; set; }
    }
}
