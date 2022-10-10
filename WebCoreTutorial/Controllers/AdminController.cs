using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCoreTutorial.Data;

namespace WebCoreTutorial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDb db;
        public static string Message { get; set; }
        public static string successMsg { get; set; }


        public AdminController(ApplicationDb _db)
        {
            db = _db;
        }

        public IActionResult Home()
        {
            return View();
        }
    }
}