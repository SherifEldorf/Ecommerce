using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCoreTutorial.Data;
using WebCoreTutorial.Models;

namespace WebCoreTutorial.Controllers
{
    public class BillingAddressesController : Controller
    {
        private readonly ApplicationDb _context;

        public BillingAddressesController(ApplicationDb context)
        {
            _context = context;
        }

        // GET: BillingAddresses
        public async Task<IActionResult> Index()
        {
            return View(await _context.BillingAddresses.ToListAsync());
        }

        // GET: BillingAddresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billingAddress = await _context.BillingAddresses
                .FirstOrDefaultAsync(m => m.id == id);
            if (billingAddress == null)
            {
                return NotFound();
            }

            return View(billingAddress);
        }

        // GET: BillingAddresses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BillingAddresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,firstName,lastName,UserName,Email,Address,Country,Zip")] BillingAddress billingAddress)
        {
            if (ModelState.IsValid)
            {
                _context.Add(billingAddress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(billingAddress);
        }

        // GET: BillingAddresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billingAddress = await _context.BillingAddresses.FindAsync(id);
            if (billingAddress == null)
            {
                return NotFound();
            }
            return View(billingAddress);
        }

        // POST: BillingAddresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,firstName,lastName,UserName,Email,Address,Country,Zip")] BillingAddress billingAddress)
        {
            if (id != billingAddress.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(billingAddress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillingAddressExists(billingAddress.id))
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
            return View(billingAddress);
        }

        // GET: BillingAddresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billingAddress = await _context.BillingAddresses
                .FirstOrDefaultAsync(m => m.id == id);
            if (billingAddress == null)
            {
                return NotFound();
            }

            return View(billingAddress);
        }

        // POST: BillingAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var billingAddress = await _context.BillingAddresses.FindAsync(id);
            _context.BillingAddresses.Remove(billingAddress);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillingAddressExists(int id)
        {
            return _context.BillingAddresses.Any(e => e.id == id);
        }
    }
}
