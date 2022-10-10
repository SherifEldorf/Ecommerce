using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCoreTutorial.Data;
using WebCoreTutorial.Models;

namespace WebCoreTutorial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SiteSettingsController : Controller
    {
        private readonly ApplicationDb db;

        public SiteSettingsController(ApplicationDb _db)
        {
            db = _db;
        }

        public async Task<IActionResult> Index()
        {
            if (db.UserSettings.Count() < 1)
            {
                await InsertUserSetting();
            }
            return View();
        }

        // Register Settings **************************************
        public async Task InsertUserSetting()
        {
            UserSetting userSetting = new UserSetting();
            userSetting.isEmailConfirm = true;
            userSetting.isRegisterOpen = true;
            userSetting.MinimumPassLength = 1;
            userSetting.MaxPassLength = 25;
            userSetting.isDigit = false;
            userSetting.isUpper = false;
            userSetting.SendWelcomeMessage = false;
            db.Add(userSetting);
            await db.SaveChangesAsync();
        }

        public static async Task<DataTable> GetUserSetting()
        {
            DataTable dt = new DataTable();
            DataAccessLayer dal = new DataAccessLayer();
            try
            {
                dt = dal.SelectData("GetUserSetting", null);
            }
            catch { }
            return await Task.FromResult(dt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRegSetting(int id, [Bind("id,isEmailConfirm,isRegisterOpen,MinimumPassLength,MaxPassLength,isDigit,isUpper,SendWelcomeMessage")] UserSetting userSetting)
        {
            userSetting.id = 1;
            if (id != userSetting.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(userSetting);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserSettingExists(userSetting.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserSettingExists(int id)
        {
            return db.UserSettings.Any(e => e.id == id);
        }

        // End Register Settings **************************************

    }
}