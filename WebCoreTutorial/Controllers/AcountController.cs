using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using WebCoreTutorial.Data;
using WebCoreTutorial.Models;

namespace WebCoreTutorial.Controllers
{
    public class AcountController : Controller
    {
        private readonly ApplicationDb db;
        private readonly IHostingEnvironment host;
        public static string Message { get; set; }
        public static string successMsg { get; set; }
        public static bool IsProfileExist { get; set; }
        public static bool RegisterOpen { get; set; }

        public static long UID { get; set; }
        public static string oldImage { get; set; }


        public AcountController(ApplicationDb _db, IHostingEnvironment _host)
        {
            db = _db;
            host = _host;
        }

        public IActionResult Register()
        {
            if (db.UserSettings.Count() > 0)
            {
                if (IsRegisterOpen() == "true")
                {
                    RegisterOpen = true;
                }
                else
                {
                    RegisterOpen = false;
                }
            }
            else
            {
                RegisterOpen = true;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("id, UserName, Email, Password, PasswordConfirm, Phone, EmailConfirm")] AppUser appUser)
        {
            Message = string.Empty;
            successMsg = string.Empty;

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(IsEmailConfirm()))
                {
                    if (IsEmailConfirm() == "true")
                    {
                        appUser.EmailConfirm = true;
                    }
                    else
                    {
                        appUser.EmailConfirm = false;
                    }
                }
                else
                {
                    appUser.EmailConfirm = false;
                }

                string input = appUser.Password;
                if (!string.IsNullOrEmpty(input))
                {
                    if (PasswordMinimumLength() > 0 && PasswordMaximumLength() > 0)
                    {
                        int min = PasswordMinimumLength();
                        int max = PasswordMaximumLength();
                        if (input.Length < min)
                        {
                            Message = "الخد الأدني لعدد احرف الباسوورد " + min + " مقاطع";
                            return View();
                        }
                        if (input.Length > max)
                        {
                            Message = "الخد الأعلي لعدد احرف الباسوورد " + max + " مقاطع";
                            return View();
                        }
                    }

                    if (!string.IsNullOrEmpty(IsPasswordDigit()))
                    {
                        string isdigit = IsPasswordDigit();
                        if (isdigit == "true")
                        {
                            if (!input.Any(char.IsDigit))
                            {
                                Message = "يجب ارفاق علي الاقل رقم واحد بكلمة المرور";
                                return View();
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(IsPasswordUpper()))
                    {
                        string isupper = IsPasswordUpper();
                        if (isupper == "true")
                        {
                            if (!input.Any(char.IsUpper))
                            {
                                Message = "يجب ارفاق علي الاقل حرف كابيتال بكلمة المرور";
                                return View();
                            }
                        }
                    }


                    appUser.Password = AppHash.HashPassword(input);
                    appUser.PasswordConfirm = AppHash.HashPassword(input);

                    DataTable dt = new DataTable();
                    Users users = new Users();
                    string userName = appUser.UserName;
                    string email = appUser.Email;
                    dt = users.CheckUserNameExist(userName);

                    if (dt.Rows.Count < 1)
                    {
                        if (!IsEmailAddressExist(email))
                        {
                            int userCount = db.AppUsers.Count();

                            db.Add(appUser);
                            string userId = appUser.id;
                            await db.SaveChangesAsync();

                            string title = "تاكيد اشتراكك بموقع التجربة";
                            string body = "مرحبا " + userName + "<br />";
                            body += "يرجي الضغط علي الرابط ادناه لتفعيل اشتراكك بموق التجربة" + "<br />" + "<br />";
                            body += "https://localhost:44313/Acount/AccountValidate?UId=" + userId;
                            if (SendEmail(email, body, title))
                            {
                                if (await InsertEmailConfirm(userId))
                                {
                                    successMsg = "تم انشاء حسابك بنجاح يرجي زيارة بريدك الالكتروني لتفعيل حسابك";
                                    if (!string.IsNullOrEmpty(IsSendEmailAfterRegister()))
                                    {
                                        if (IsSendEmailAfterRegister() == "true")
                                        {
                                            title = "شكرا لتسجيلك معنا بموقع التجربة";
                                            body = "مرحبا " + userName + "<br />";
                                            body += "شكرا لتسجيلك معنا بموقع التجربة";
                                            SendEmail(email, body, title);
                                        }
                                    }
                                }
                                else
                                {
                                    Message = "خطأ بعملية اضافة الحساب, يرجي المحاولة لاحقا";
                                }
                            }
                            else
                            {
                                if (await InsertEmailConfirm(userId))
                                {
                                    Message = "تم انشاء حسابك بنجاح وتعذر ارسال رسالة التفعيل الي بريدك الالكتروني";
                                }
                            }

                            string roleId = string.Empty;
                            if (userCount <= 0)
                            {
                                AppRole appRole = new AppRole();
                                appRole.RoleName = "Admin";
                                await db.AddAsync(appRole);
                                await db.SaveChangesAsync();
                                roleId = appRole.id;

                                appRole.id = Guid.NewGuid().ToString();
                                appRole.RoleName = "SuperVisor";
                                await db.AddAsync(appRole);
                                await db.SaveChangesAsync();

                                appRole.id = Guid.NewGuid().ToString();
                                appRole.RoleName = "Member";
                                await db.AddAsync(appRole);
                                await db.SaveChangesAsync();

                                UserRole userRole = new UserRole();
                                userRole.RoleId = roleId;
                                userRole.UserId = userId;
                                await db.AddAsync(userRole);
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                roleId = Data.AppAuthentication.GetRoleId("Member");
                                if (!string.IsNullOrEmpty(roleId))
                                {
                                    UserRole userRole = new UserRole();
                                    userRole.RoleId = roleId;
                                    userRole.UserId = userId;
                                    await db.AddAsync(userRole);
                                    await db.SaveChangesAsync();
                                }
                            }

                            return RedirectToAction(nameof(Register));
                        }
                        else
                        {
                            Message = "البريد الالكتروني المدخل (" + email + ") غير متوفر";
                            return View();
                        }
                    }
                    else
                    {
                        Message = "اسم المستخدم المدخل (" + userName + ") غير متوفر";
                        return View();
                    }
                }
            }

            return RedirectToAction(nameof(Register));
        }

        public bool IsEmailAddressExist(string email)
        {
            return db.AppUsers.Any(a => a.Email == email);
        }

        public async Task<bool> InsertEmailConfirm(string userId)
        {
            try
            {
                AppConfirm app = new AppConfirm();
                app.UserId = userId;
                app.DateConfirm = DateTime.Now;
                db.Add(app);
                string id = app.id;
                await db.SaveChangesAsync();
                return true;
            }
            catch { }

            return false;
        }

        public bool SendEmail(string email, string body, string title)
        {
            try
            {
                MailMessage ms = new MailMessage("abuadam053@gmail.com", email);
                ms.Subject = title;
                ms.Body = body;
                ms.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.Credentials = new NetworkCredential()
                {
                    UserName = "abuadam053@gmail.com",
                    Password = ""
                };

                client.EnableSsl = true;
                client.Send(ms);

                return true;
            }
            catch { }

            return false;
        }

        public async Task<IActionResult> AccountValidate()
        {
            Message = string.Empty;
            successMsg = string.Empty;
            bool isForgetPassword = false;

            string id = HttpContext.Request.Query["UId"].ToString();
            if (string.IsNullOrEmpty(id))
            {
                id = HttpContext.Request.Query["PId"].ToString();
                isForgetPassword = true;
                if (string.IsNullOrEmpty(id))
                    return NotFound();
            }

            DataTable dt = new DataTable();
            Users users = new Users();
            dt = users.CheckEmailConfirmExist(id);
            if (dt.Rows.Count > 0)
            {
                try
                {
                    string appConfrimId = dt.Rows[0][1].ToString();
                    DateTime dateConfrim = DateTime.Parse(dt.Rows[0][2].ToString());
                    string userId = dt.Rows[0][1].ToString();

                    if (dateConfrim.AddHours(24) > DateTime.Now)
                    {
                        if (!isForgetPassword)
                        {
                            await Task.Run(() =>
                            {
                                users.UpdateEmailConfirm(userId, true);
                            });

                            users.DeleteEmailConfirm(appConfrimId);
                        }
                        else
                        {
                            string pass = GeneratePassword(10);
                            if (await PasswordReset(userId, pass))
                            {
                                var user = await db.AppUsers.FirstOrDefaultAsync(s => s.id == id);
                                if (user != null)
                                {
                                    string title = "كلمة المرور الجديدة بموقع التجربة";
                                    string body = "مرحبا " + user.UserName + "<br />";
                                    body += "نم توليد كلمة مرور جديدة حسب طلبك بموقع التجربة" + "<br />" + "<br />";
                                    body += "كللمة المرور الجديدة هي :" + "<br />";
                                    body += pass;

                                    if (SendEmail(user.Email, body, title))
                                    {
                                        users.DeleteEmailConfirm(appConfrimId);
                                        TempData["successPass"] = "تم ارسال كلمة المرور الجديدة الي بريدك الالكتروني بنجاح";
                                        RedirectToAction(nameof(Login));
                                    }
                                }
                            }
                        }

                        if (users.state)
                        {
                            successMsg = "شكرا لتسجيلك معنا لقد اتممت تفعيل اشتراكك" + "\r\n";
                            successMsg += "بامكانك الذهاب لتعديل بياناتك الشخصية او الذهب للصفحة الرئيسية";
                        }
                    }
                    else
                    {
                        Message = "للأسف انتهت صلاحية رابط اشتراكك وهي 24 ساعة";
                        users.DeleteEmailConfirm(appConfrimId);
                    }
                }
                catch { }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public class LoginModel
        {
            [StringLength(250), Required(ErrorMessage = "اسم المستخدم مطلوب"), Display(Name = "اسم المستخدم")]
            public string UserName { get; set; }

            [StringLength(650), Required(ErrorMessage = "كلمة المرور مطلوبة"), Display(Name = "كلمة المرور"), DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "تذكرني")]
            public bool rememberMe { get; set; }
        }

        [BindProperty]
        public LoginModel input { get; set; }

        public ActionResult Login()
        {
            var msg = TempData["successPass"] as string;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string username, string password, bool rem)
        {
            if (username == null || password == null)
            {
                return View();
            }

            if (IsLoged(username, password))
            {
                string id = AppAuthentication.GetIdByUserName(username);
                if (!string.IsNullOrEmpty(id))
                {
                    var appUser = await db.AppUsers.FindAsync(id);
                    if (appUser != null)
                    {
                        if (appUser.Lockout == false)
                        {
                            appUser.ErrorLogCount = 0;
                            db.AppUsers.Attach(appUser);
                            db.Entry(appUser).Property(x => x.ErrorLogCount).IsModified = true;
                            await db.SaveChangesAsync();

                            AddCookies(username, AppAuthentication.GetRoleName(username), password, rem);

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            if (await IsLuckoutFinished(appUser.LockTime, id))
                            {
                                AddCookies(username, AppAuthentication.GetRoleName(username), password, rem);
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewBag.msg = "تم حظر هذا الحساب مؤقتا يرجي معاودة محاولة تسجيل الدخول بعد انقضاء مدة الحظر";
                                return View();
                            }

                        }
                    }
                }
            }
            else
            {
                if (await logError(username))
                {
                    ViewBag.msg = "نظرا لمحاولات التسجيل المتكررة والخاطئة تم اغلاق حساب " + username + " لمدة 12 ساعة";
                }
            }
            return View();
        }

        private bool IsLoged(string username, string password)
        {
            DataTable dt = new DataTable();
            Users cs = new Users();
            string hash = AppHash.HashPassword(password);
            dt = cs.CheckLogin(username, hash);
            if (dt.Rows.Count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> logError(string username)
        {
            string id = AppAuthentication.GetIdByUserName(username);
            if (!string.IsNullOrEmpty(id))
            {
                var appUser = await db.AppUsers.FindAsync(id);
                if (appUser != null)
                {
                    appUser.ErrorLogCount += 1;
                    int count = appUser.ErrorLogCount;

                    if (appUser.ErrorLogCount < 5)
                    {
                        db.AppUsers.Attach(appUser);
                        await db.SaveChangesAsync();

                        ViewBag.msg = "بيانات الدخول غير صحيحة !!!" + "\r\n" + "لديك ( " + count + " ) محاولة تسجيل دخول خاطئة من عدد " + "(5) محاولات";
                        return false;
                    }
                    else
                    {
                        db.AppUsers.Attach(appUser);
                        appUser.ErrorLogCount += 1;
                        appUser.LockTime = DateTime.Now.AddHours(12);
                        appUser.Lockout = true;
                        db.Entry(appUser).Property(x => x.Lockout).IsModified = true;
                        db.Entry(appUser).Property(x => x.LockTime).IsModified = true;
                        db.Entry(appUser).Property(x => x.ErrorLogCount).IsModified = true;
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task<bool> IsLuckoutFinished(DateTime? lockDate, string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var appUser = await db.AppUsers.FindAsync(id);
                if (appUser != null)
                {
                    if (lockDate != null)
                    {
                        if (DateTime.Now >= lockDate)
                        {
                            appUser.ErrorLogCount = 0;
                            appUser.LockTime = null;
                            appUser.Lockout = false;
                            db.AppUsers.Attach(appUser);
                            db.Entry(appUser).Property(x => x.Lockout).IsModified = true;
                            db.Entry(appUser).Property(x => x.LockTime).IsModified = true;
                            db.Entry(appUser).Property(x => x.ErrorLogCount).IsModified = true;
                            await db.SaveChangesAsync();

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public async void AddCookies(string username, string roleName, string password, bool remember)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, AppAuthentication.GetIdByUserName(username)),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("password", password),
                new Claim(ClaimTypes.IsPersistent, remember.ToString())
            };

            var claimIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);

            if (remember)
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = remember,
                    ExpiresUtc = DateTime.UtcNow.AddDays(10)
                };

                await HttpContext.SignInAsync
                (
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimIdentity),
                   authProperties
                );
            }
            else
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = remember,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync
                (
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimIdentity),
                   authProperties
                );
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> UserControl()
        {
            string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id == null)
            {
                return NotFound();
            }

            var user = await db.AppUsers.FirstOrDefaultAsync(i => i.id == id);
            if (user == null)
            {
                return NotFound();
            }

            if (UserIdProfileExists(id))
            {
                IsProfileExist = true;
            }
            else
            {
                IsProfileExist = false;
            }

            long profileId = GetUserProfileId(id);
            if (profileId > 0)
            {
                UID = profileId;
            }
            else
            {
                UID = 0;
            }

            return View(user);
        }

        [Authorize]
        public IActionResult Profile(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.img = host.WebRootPath;

            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([Bind("id,Country,UserId,DateOfBurth,PersonalWebUrl,UrlImage")] UserProfile userProfile, IFormFile img)
        {
            ViewBag.msg = string.Empty;

            string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id == null)
            {
                return NotFound();
            }

            string newFileName = string.Empty;
            if (img != null && img.Length > 0)
            {
                string fn = img.FileName;
                if (IsImageValidate(fn))
                {
                    string extension = Path.GetExtension(fn);
                    newFileName = Guid.NewGuid().ToString() + extension;
                    string filename = Path.Combine(host.WebRootPath + "/images/user", newFileName);
                    await img.CopyToAsync(new FileStream(filename, FileMode.Create));
                }
                else
                {
                    ViewBag.msg = "الملفات المسموح بها : png, jpeg, jpg, gif, bmp";
                    return View();
                }
            }

            if (string.IsNullOrEmpty(userProfile.UserId) && string.IsNullOrEmpty(userProfile.Country) &&
                userProfile.DateOfBurth == null && string.IsNullOrEmpty(userProfile.PersonalWebUrl) &&
                string.IsNullOrEmpty(userProfile.UrlImage))
            {
                ViewBag.msg = "لم تقم باي اخيار !!!";
                return View();
            }

            userProfile.UserId = id;
            userProfile.UrlImage = newFileName;
            db.Add(userProfile);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(UserControl));
        }

        private bool IsImageValidate(string filename)
        {
            string extension = Path.GetExtension(filename);
            if (extension.Contains(".png"))
                return true;

            if (extension.Contains(".jpeg"))
                return true;

            if (extension.Contains(".jpg"))
                return true;

            if (extension.Contains(".gif"))
                return true;

            if (extension.Contains(".bmp"))
                return true;

            return false;
        }

        [Authorize]
        public async Task<IActionResult> EditProfile(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var userProfile = await db.UserProfiles.FindAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }

            oldImage = userProfile.UrlImage;

            return View(userProfile);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(long id, [Bind("id,Country,UserId,DateOfBurth,PersonalWebUrl,UrlImage")] UserProfile userProfile, IFormFile img)
        {
            if (id != userProfile.id)
            {
                return NotFound();
            }

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    string newFileName = string.Empty;
                    if (img != null && img.Length > 0)
                    {
                        string fn = img.FileName;
                        if (IsImageValidate(fn))
                        {
                            string extension = Path.GetExtension(fn);
                            newFileName = Guid.NewGuid().ToString() + extension;
                            string filename = Path.Combine(host.WebRootPath + "/images/user", newFileName);
                            await img.CopyToAsync(new FileStream(filename, FileMode.Create));
                            ViewBag.msg = "";
                        }
                        else
                        {
                            ViewBag.msg = "الملفات المسموح بها : png, jpeg, jpg, gif, bmp";
                            return View();
                        }
                    }
                    else
                    {
                        newFileName = oldImage;
                    }

                    userProfile.UrlImage = newFileName;
                    userProfile.UserId = userId;
                    db.Update(userProfile);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(UserControl));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProfileExists(userProfile.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }


            return View(userProfile);
        }

        private bool UserProfileExists(long id)
        {
            return db.UserProfiles.Any(e => e.id == id);
        }

        private bool UserIdProfileExists(string userId)
        {
            return db.UserProfiles.Any(e => e.UserId == userId);
        }

        private long GetUserProfileId(string userId)
        {
            try
            {
                long id = db.UserProfiles.Where(e => e.UserId == userId).Select(e => e.id).FirstOrDefault();
                return id;
            }
            catch { }
            return 0;
        }

        private string GetUserPassword(string userId)
        {
            try
            {
                return db.AppUsers.Where(e => e.id == userId).Select(e => e.Password).FirstOrDefault();
            }
            catch { }
            return string.Empty;
        }

        [Authorize]
        [HttpPost]
        public ActionResult verifyPassword(string pass)
        {
            if (string.IsNullOrEmpty(pass))
            {
                return RedirectToAction(nameof(UserControl));
            }

            string password = AppHash.HashPassword(pass);
            string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(id))
            {
                if (password == GetUserPassword(id))
                {
                    Message = string.Empty;
                    return RedirectToAction(nameof(ChangePassword), new { UId = password });
                }
                else
                {
                    Message = "كلمة المرور المدخلة غير صحيحة!!!";
                }
            }

            return RedirectToAction(nameof(UserControl));
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            string uid = HttpContext.Request.Query["UId"].ToString();
            if (!string.IsNullOrEmpty(uid))
            {
                return RedirectToAction("Index", "Home");
            }

            string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }

            if (GetUserPassword(id) != uid)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ChangePassword(string pass, string passConfirm)
        {
            if (pass == null || passConfirm == null)
            {
                return RedirectToAction(nameof(UserControl));
            }

            string password = AppHash.HashPassword(pass);
            string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(id))
            {
                if (pass == passConfirm)
                {
                    var user = await db.AppUsers.FirstOrDefaultAsync(a => a.id == id);
                    if (user != null)
                    {
                        user.Password = password;
                        user.PasswordConfirm = password;
                        db.Attach(user);
                        db.Entry(user).Property(x => x.Password).IsModified = true;
                        db.Entry(user).Property(x => x.PasswordConfirm).IsModified = true;
                        await db.SaveChangesAsync();
                        return RedirectToAction(nameof(UserControl));
                    }
                }
                else
                {
                    ViewBag.msg = "كلمتا المرور غير متطابقتين!!";
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult ForgetPassword()
        {
            var msg = TempData["success"] as string;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgetPassword(string email)
        {
            if (email == null)
            {
                return View();
            }

            if (IsEmailAddressExist(email))
            {
                var user = await db.AppUsers.FirstOrDefaultAsync(e => e.Email == email);
                if (user != null)
                {
                    string title = "استعادة كلمة المرور بموقع التجربة";
                    string body = "مرحبا بك " + user.UserName + "<br />";
                    body += "يرجي الضغط علي الرابط ادناه لتفعيل طلب استعادة كلمة المرور بموقع التجربة" + "<br />" + "<br />";
                    body += "https://localhost:44313/Acount/AccountValidate?PId=" + user.id;
                    if (SendEmail(email, body, title))
                    {
                        if (await InsertEmailConfirm(user.id))
                        {
                            ViewBag.msg = "";
                            TempData["success"] = "تم ارسال طلب استعادة كلمة المرور الي بريدك الالكتروني بنجاح";
                            RedirectToAction(nameof(ForgetPassword));
                        }
                        else
                        {
                            ViewBag.msg = "حطأ بعملية حفظ البيانات";
                        }
                    }
                    else
                    {
                        ViewBag.msg = "خطأ بعملية ارسال رسالة التفعيل الي بريدك الالكتروني";
                    }
                }
            }
            else
            {
                ViewBag.msg = "البريد الالكتروني غير مستعمل ...";
            }

            return View();
        }

        private string GeneratePassword(int stringLength)
        {
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
            char[] chars = new char[stringLength];
            Random rd = new Random();
            for (int i = 0; i < stringLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public async Task<bool> PasswordReset(string id, string password)
        {
            try
            {
                var user = await db.AppUsers.FirstOrDefaultAsync(s => s.id == id);
                if (user != null)
                {
                    string pass = AppHash.HashPassword(password);
                    user.Password = pass;
                    user.PasswordConfirm = pass;
                    db.Attach(user);
                    db.Entry(user).Property(x => x.Password).IsModified = true;
                    db.Entry(user).Property(x => x.PasswordConfirm).IsModified = true;
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            catch { }

            return false;
        }

        private string IsEmailConfirm()
        {
            try
            {
                return db.UserSettings.Where(e => e.id == 1).Select(e => e.isEmailConfirm).FirstOrDefault().ToString().ToLower();
            }
            catch { }
            return string.Empty;
        }

        private string IsRegisterOpen()
        {
            try
            {
                return db.UserSettings.Where(e => e.id == 1).Select(e => e.isRegisterOpen).FirstOrDefault().ToString().ToLower();
            }
            catch { }
            return string.Empty;
        }

        private int PasswordMinimumLength()
        {
            try
            {
                int i = 0;
                i = db.UserSettings.Where(e => e.id == 1).Select(e => e.MinimumPassLength).FirstOrDefault();
                return i;
            }
            catch { }
            return 0;
        }

        private int PasswordMaximumLength()
        {
            try
            {
                int i = 0;
                i = db.UserSettings.Where(e => e.id == 1).Select(e => e.MaxPassLength).FirstOrDefault();
                return i;
            }
            catch { }
            return 0;
        }

        private string IsPasswordDigit()
        {
            try
            {
                return db.UserSettings.Where(e => e.id == 1).Select(e => e.isDigit).FirstOrDefault().ToString().ToLower();
            }
            catch { }
            return string.Empty;
        }

        private string IsPasswordUpper()
        {
            try
            {
                return db.UserSettings.Where(e => e.id == 1).Select(e => e.isUpper).FirstOrDefault().ToString().ToLower();
            }
            catch { }
            return string.Empty;
        }

        private string IsSendEmailAfterRegister()
        {
            try
            {
                return db.UserSettings.Where(e => e.id == 1).Select(e => e.SendWelcomeMessage).FirstOrDefault().ToString().ToLower();
            }
            catch { }
            return string.Empty;
        }
    }
}