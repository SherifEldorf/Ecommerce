using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCoreTutorial.Data;
using WebCoreTutorial.Models;

namespace WebCoreTutorial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDb db;
        public static int cartCount = 0;
        public HomeController(ApplicationDb _db)
        {
            db = _db;
        }

        public int CartCount()
        {
            int i = 0;
            string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id != null)
                i = db.Carts.Where(x => x.UserId == id).Count();
            else
                i = 0;

            return i;
        }

        public async Task<IActionResult> Index()
        {
            var post = db.Posts.OrderByDescending(x => x.id).Include(e => e.SubCategory);
            cartCount = CartCount();
            return View(await post.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> PostView(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await db.Posts
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(m => m.id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart([Bind("id,ProductName,Price,Discount")] Cart cart,
            string proName, double? price, int? discount, long postId)
        {
            string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id == null)
            {
                TempData["msg"] = "انت غير مسجل معنا";
                return RedirectToAction("PostView", new { id = postId });
            }
            if (!string.IsNullOrEmpty(proName) && price != null)
            {
                if (!GetCartProductName(proName, id))
                {
                    cart.ProductName = proName;
                    cart.Price = price;
                    cart.Discount = discount;
                    cart.UserId = id;

                    db.Add(cart);
                    await db.SaveChangesAsync();
                    cartCount = CartCount();
                }
                else
                {
                    TempData["msg"] = "اسم المنتج: (" + proName + ") موجود مسبقا بقائمتك";
                }
            }

            return RedirectToAction("PostView", new { id = postId });
        }

        [Authorize]
        public async Task<IActionResult> Cart(string id)
        {
            if (id != null)
            {
                ViewBag.Cart = await GetCart(id);
                ViewBag.Bill = await GetBill(id);
                ViewBag.Pay = await GetPayment(id);
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetCartId(int? id, string userId)
        {
            if (id != null)
            {
                TempData["BillingId"] = id;
            }
            return RedirectToAction("Cart", new { id = userId });
        }

        public async Task<List<Cart>> GetCart(string userId)
        {
            return await db.Carts.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<List<Payment>> GetPayment(string userId)
        {
            return await db.Payments.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<List<BillingAddress>> GetBill(string userId)
        {
            return await db.BillingAddresses.Where(x => x.UserId == userId).ToListAsync();
        }

        public bool GetCartProductName(string proName, string id)
        {
            return db.Carts.Where(s => s.UserId == id).Any(x => x.ProductName == proName);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await db.Carts.FirstOrDefaultAsync(m => m.id == id);
            if (cart == null)
            {
                return NotFound();
            }

            db.Carts.Remove(cart);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Cart));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckOut(
            int billId, string firstname, string lastname, string username, string email, string address, string country, int zip,
            string cardtype, string cardname, long cardnumber, DateTime expire, int cvv, int cartId, string userId)
        {
            if (!UserExists(userId))
            {
                return RedirectToAction(nameof(Index));
            }
            if (!CartExists(cartId))
            {
                TempData["fail"] = "يجب ملئ جميع البيانات بمربعات النص";
                return RedirectToAction("Cart", new { id = userId });
            }

            bool execute = false;
            username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (firstname != null && lastname != null && username != null && email != null && address != null && country != null && zip > 0)
            {
                if (db.BillingAddresses.Where(x => x.UserId == userId).Count() < 1)
                {
                    BillingAddress bill = new BillingAddress();
                    bill.Address = address;
                    bill.Country = country;
                    bill.Email = email;
                    bill.firstName = firstname;
                    bill.lastName = lastname;
                    bill.UserName = username;
                    bill.Zip = zip;
                    bill.UserId = userId;

                    db.Add(bill);
                    await db.SaveChangesAsync();
                    billId = bill.id;
                    execute = true;
                }
                else
                {
                    var bill = await db.BillingAddresses.FirstOrDefaultAsync(x => x.id == billId && x.UserId == userId);
                    if (bill != null)
                    {
                        billId = bill.id;
                        if (bill.Address != address ||
                            bill.Country != country ||
                            bill.Email != email ||
                            bill.firstName != firstname ||
                            bill.lastName != lastname ||
                            bill.UserName != username ||
                            bill.Zip != zip)
                        {
                            bill.Address = address;
                            bill.Country = country;
                            bill.Email = email;
                            bill.firstName = firstname;
                            bill.lastName = lastname;
                            bill.UserName = username;
                            bill.Zip = zip;

                            db.BillingAddresses.Attach(bill);
                            db.Entry(bill).Property(x => x.Address).IsModified = true;
                            db.Entry(bill).Property(x => x.Country).IsModified = true;
                            db.Entry(bill).Property(x => x.Email).IsModified = true;
                            db.Entry(bill).Property(x => x.firstName).IsModified = true;
                            db.Entry(bill).Property(x => x.lastName).IsModified = true;
                            db.Entry(bill).Property(x => x.UserName).IsModified = true;
                            db.Entry(bill).Property(x => x.Zip).IsModified = true;
                            db.Entry(bill).Property(x => x.UserId).IsModified = false;

                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
            else
            {
                TempData["fail"] = "يجب ملئ جميع البيانات بمربعات النص";
                return RedirectToAction("Cart", new { id = userId });
            }

            if (!BillExists(billId))
            {
                TempData["fail"] = "يجب ملئ جميع البيانات بمربعات النص";
                return RedirectToAction("Cart", new { id = userId });
            }

            if (execute || db.BillingAddresses.Where(x => x.UserId == userId).Count() > 0)
            {
                if (cardtype != null && cardname != null && cardnumber > 0 && expire != null && cvv > 0)
                {
                    Payment pay = new Payment();
                    pay.billingId = billId;
                    pay.cardName = cardname;
                    pay.cardNumber = cardnumber;
                    pay.cardType = cardtype;
                    pay.cartId = cartId;
                    pay.cvv = cvv;
                    pay.expiration = expire;
                    pay.UserId = userId;

                    db.Add(pay);
                    await db.SaveChangesAsync();
                    TempData["success"] = "تم حفظ جميع البيانات بنجاح";
                }
                else
                {
                    TempData["fail"] = "يجب ملئ جميع البيانات بمربعات النص";
                    return RedirectToAction("Cart", new { id = userId });
                }
            }

            return RedirectToAction("Cart", new { id = userId });
        }

        private bool CartExists(int id)
        {
            return db.Carts.Any(e => e.id == id);
        }

        private bool BillExists(int id)
        {
            return db.BillingAddresses.Any(e => e.id == id);
        }

        private bool UserExists(string id)
        {
            return db.AppUsers.Any(e => e.id == id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
