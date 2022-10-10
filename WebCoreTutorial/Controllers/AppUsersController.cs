using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebCoreTutorial.Data;
using WebCoreTutorial.Models;

namespace WebCoreTutorial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AppUsersController : Controller
    {
        private readonly ApplicationDb _context;
        public static string Message { get; set; }
        public static string successMsg { get; set; }

        public static string pass { get; set; }
        public static string user { get; set; }
        public static string email { get; set; }

        public AppUsersController(ApplicationDb context)
        {
            _context = context;
        }

        // GET: AppUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.AppUsers.ToListAsync());
        }

        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // GET: AppUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,UserName,Email,Password,PasswordConfirm,Phone,EmailConfirm")] AppUser appUser)
        {
            Message = string.Empty;
            successMsg = string.Empty;

            if (ModelState.IsValid)
            {
                string input = appUser.Password;
                if (!string.IsNullOrEmpty(input))
                {
                    DataTable dt = new DataTable();
                    Users users = new Users();
                    string userName = appUser.UserName;
                    string email = appUser.Email;
                    dt = users.CheckUserNameExist(userName);

                    if (dt.Rows.Count < 1)
                    {
                        if (!IsEmailAddressExist(email))
                        {
                            appUser.Password = AppHash.HashPassword(input);
                            appUser.PasswordConfirm = AppHash.HashPassword(input);
                            _context.Add(appUser);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            Message = "البريد الالكتروني المدخل (" + email + ") مستعمل";
                            return View();
                        }
                    }
                    else
                    {
                        Message = "اسم المستخدم المدخل (" + userName + ") مستعمل";
                        return View();
                    }
                }
            }
            return View(appUser);
        }

        public bool IsEmailAddressExist(string email)
        {
            return _context.AppUsers.Any(a => a.Email == email);
        }

        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            pass = appUser.Password;
            email = appUser.Email;
            user = appUser.UserName;
            ViewBag.pass = pass;
            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("id,UserName,Email,Password,PasswordConfirm,Phone,EmailConfirm")] AppUser appUser)
        {
            if (id != appUser.id)
            {
                return NotFound();
            }

            Message = string.Empty;
            successMsg = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    string input = appUser.Password;
                    if (input != pass)
                    {
                        appUser.Password = AppHash.HashPassword(input);
                        appUser.PasswordConfirm = AppHash.HashPassword(input);
                    }

                    DataTable dt = new DataTable();
                    Users users = new Users();

                    if(user != appUser.UserName)
                    {
                        dt = users.CheckUserNameExist(appUser.UserName);
                        if (dt.Rows.Count > 0)
                        {
                            Message = "اسم المستخدم المدخل (" + appUser.UserName + ") مستعمل";
                            return View();
                        }
                    }

                    if (email != appUser.Email)
                    {
                        if (IsEmailAddressExist(email))
                        {
                            Message = "البريد الالكتروني المدخل (" + appUser.Email + ") مستعمل";
                            return View();
                        }
                    }

                    _context.Update(appUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.id))
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
            return View(appUser);
        }

        // GET: AppUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var appUser = await _context.AppUsers.FindAsync(id);
            _context.AppUsers.Remove(appUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUserExists(string id)
        {
            return _context.AppUsers.Any(e => e.id == id);
        }
    }
}
