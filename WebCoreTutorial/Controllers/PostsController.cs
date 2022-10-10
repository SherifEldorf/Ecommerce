using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCoreTutorial.Data;
using WebCoreTutorial.Models;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;

namespace WebCoreTutorial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PostsController : Controller
    {
        private readonly ApplicationDb _context;
        private readonly IHostingEnvironment host;

        public PostsController(ApplicationDb context, IHostingEnvironment _host)
        {
            _context = context;
            host = _host;
        }

        // GET: Posts
        [HttpGet]
        public async Task<IActionResult> Index(string search, int? page)
        {
            var applicationDb = new object();
            var pageNumber = page ?? 1;
            if (!string.IsNullOrEmpty(search))
            {
                applicationDb = await _context.Posts.Include(p => p.SubCategory).Where(x => x.Auther.Contains(search) || x.Title.Contains(search)).ToList().ToPagedListAsync(pageNumber, 10);
            }
            else
            {
                applicationDb = await _context.Posts.OrderByDescending(x => x.id).Include(p => p.SubCategory).ToList().ToPagedListAsync(pageNumber, 10);
            }
            return View(applicationDb);
        }

        [HttpPost]
        public IActionResult Index(IEnumerable<string> ID)
        {
            ViewBag.msg = string.Empty;
            try
            {
                List<string> st = ID.ToList();
                if (st.Count > 0)
                {
                    foreach (var id in st)
                    {
                        long deleteId = 0;
                        try
                        {
                            deleteId = long.Parse(id);
                        }
                        catch { }

                        if (deleteId > 0)
                        {
                            var post = _context.Posts.First(m => m.id == deleteId);
                            if (post != null)
                            {
                                _context.Posts.Remove(post);
                                _context.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.msg = "لا يوجد اي اختيار بالقائمة";
                }
            }
            catch (Exception ex)
            { ViewBag.msg = ex.Message; }

            return RedirectToAction(nameof(Index));
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(m => m.id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["SubId"] = new SelectList(_context.SubCategories, "id", "SubCatName");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Title,PostContent,PostImg,Auther,PostDate,PostViews,PostLike,LikeUserName,SubId,IsPublish,ProductName,Price,Discount")] Post post, IFormFile img)
        {
            ViewBag.msg = string.Empty;
            string id = User.FindFirst(ClaimTypes.Name)?.Value;
            if (id == null)
            {
                return NotFound();
            }

            string newFileName = string.Empty;
            if (img != null && img.Length > 0)
            {
                newFileName = img.FileName;
                if (IsImageValidate(newFileName))
                {
                    string filename = Path.Combine(host.WebRootPath + "/images/Post", newFileName);
                    await img.CopyToAsync(new FileStream(filename, FileMode.Create));
                }
                else
                {
                    ViewBag.msg = "الملفات المسموح بها : png, jpeg, jpg, gif, bmp";
                    return View();
                }
            }

            try
            {
                post.Auther = id;
                post.LikeUserName = "";
                post.PostDate = DateTime.Now;
                post.PostImg = newFileName;
                post.PostLike = 0;
                post.PostViews = 0;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch { }

            ViewData["SubId"] = new SelectList(_context.SubCategories, "id", "SubCatName", post.SubId);
            return View(post);
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

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["SubId"] = new SelectList(_context.SubCategories, "id", "SubCatName", post.SubId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, string Auther, DateTime PostDate, int PostViews, int PostLike,
            string LikeUserName, string PostImg,
            [Bind("id,Title,PostContent,PostImg,Auther,PostDate,PostViews,PostLike,LikeUserName,SubId,IsPublish,ProductName,Price,Discoun")] Post post, IFormFile img)
        {
            if (id != post.id)
            {
                return NotFound();
            }

            ViewBag.msg = string.Empty;
            string newFileName = string.Empty;
            if (img != null && img.Length > 0)
            {
                newFileName = img.FileName;
                if (IsImageValidate(newFileName))
                {
                    string filename = Path.Combine(host.WebRootPath + "/images/Post", newFileName);
                    await img.CopyToAsync(new FileStream(filename, FileMode.Create));
                }
                else
                {
                    ViewBag.msg = "الملفات المسموح بها : png, jpeg, jpg, gif, bmp";
                    return View();
                }
            }

            try
            {
                try
                {
                    post.Auther = Auther;
                    post.LikeUserName = LikeUserName;
                    post.PostDate = PostDate;
                    if (!string.IsNullOrEmpty(newFileName))
                        post.PostImg = newFileName;
                    else
                        post.PostImg = PostImg;

                    post.PostLike = PostLike;
                    post.PostViews = PostViews;
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch { }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(post.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            ViewData["SubId"] = new SelectList(_context.SubCategories, "id", "SubCatName", post.SubId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(m => m.id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(long id)
        {
            return _context.Posts.Any(e => e.id == id);
        }
    }
}
