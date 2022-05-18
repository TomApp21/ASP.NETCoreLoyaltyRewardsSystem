#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP.NETCoreLoyaltyRewardsSystem.Areas.Identity.Data;
using ASP.NETCoreLoyaltyRewardsSystem.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ASP.NETCoreLoyaltyRewardsSystem.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public TransactionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var allTransactions = await _context.Transactions.ToListAsync();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userTransactions = allTransactions.Where(x => x.UserId == userId).ToList();
            return View(userTransactions);
        }

        public async Task<string> AddTransaction()
        {
            string rtnString = "";

            Random rnd = new Random();
            int amountSpent = rnd.Next(450, 2000); // creates random number

            // move into own functino
            decimal amountSpnt = (decimal)amountSpent / 100;


            using (var context = _context)
            {
                var transaction = new Transaction
                {
                    UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    DateOfTransaction = DateTime.Now,
                    AmountBeforeDiscount = amountSpent,
                    PointsApplied = 0
                };
                _context.Add(transaction);
                await context.SaveChangesAsync();


                // update user table
                ApplicationUser userModel = await _userManager.FindByIdAsync(transaction.UserId);
                //(User.Identity.GetUserId());

                userModel.AvailablePoints += Convert.ToInt32(amountSpnt * 10);
                IdentityResult result = await _userManager.UpdateAsync(userModel);

            }

            return rtnString = "You spent a total of £" + amountSpnt + " gained " + Math.Round(amountSpnt * 10) + " points!";


        }


        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,DateOfTransaction,AmountBeforeDiscount,PointsApplied")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,DateOfTransaction,AmountBeforeDiscount,PointsApplied")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
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
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
