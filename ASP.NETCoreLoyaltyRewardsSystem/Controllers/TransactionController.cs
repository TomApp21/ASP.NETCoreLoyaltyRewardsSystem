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
using ASP.NETCoreLoyaltyRewardsSystem.ViewModel;

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

            List<TransactionViewModel> transactions = new List<TransactionViewModel>();
            foreach (var transaction in userTransactions)
            {
                transactions.Add(new TransactionViewModel 
                    { PointsApplied = transaction.PointsApplied, 
                      AmountBeforeDiscount = transaction.AmountBeforeDiscount,
                      DateOfTransaction = transaction.DateOfTransaction 
                });
            }

            return View(transactions);
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

        public async Task<string> RedeemItem()
        {

            string rtnString = "";


            // update user table
            ApplicationUser userModel = await _userManager.FindByIdAsync(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            int userPoints = userModel.AvailablePoints;
            int rewardIndex = 0;


            if (userPoints < 100)
            {
                return rtnString = "Not eligible for a reward. Must have at least 100 points.";
            }
            else if (userPoints >= 100 && userPoints < 200)
            {
                rewardIndex = 1;
            }
            else if (userPoints >= 200 && userPoints < 500)
            {
                rewardIndex = 2;

            }
            else if (userPoints >= 500 && userPoints < 1000)
            {
                rewardIndex = 3;
            }
            else if (userPoints >= 1000)
            {
                rewardIndex = 4;
            }


            Random rnd = new Random();
            int reward = rnd.Next(1, rewardIndex++);

            int points = 0;


            switch(reward)
            {
                case (1):
                    rtnString = "You redeemed a free drink.";
                    points = 100;
                    break;
                case (2):
                    rtnString = "You redeemed a free snack.";
                    points = 200; 
                    break;
                case (3):
                    rtnString = "You redeemed a free sandwich.";
                    points = 500;
                    break;
                case (4):
                    rtnString = "You redeemed a free large sandwich.";
                    points = 1000;
                    break;
            }


            using (var context = _context)
            {
                var transaction = new Transaction
                {
                    UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    DateOfTransaction = DateTime.Now,
                    AmountBeforeDiscount = 0,
                    PointsApplied = points
                };
                _context.Add(transaction);
                await context.SaveChangesAsync();






                userModel.AvailablePoints -= Convert.ToInt32(points);
                IdentityResult result = await _userManager.UpdateAsync(userModel);

            }
            return rtnString;
        }



        public async Task<bool> CheckLoyaltyBonus()
        {
            string rtnString = string.Empty;
            bool blnReturn = false;


            using (var context = _context)
            {

                var allTransactions = await _context.Transactions.ToListAsync();
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userTransactions = allTransactions.Where(x => x.UserId == userId && x.PointsApplied > 0).ToList();
                //var lastRedemption = allTransactions.Where(x => x.UserId == userId && x.PointsApplied > 0).OrderByDescending(x => x.Id).FirstOrDefault();


                int totalPointsRedeemed = 0;
                bool loyaltyMember = false;

                foreach (var t in userTransactions)
                {
                    totalPointsRedeemed += t.PointsApplied;
                    if (t.PointsApplied == 1000)
                        loyaltyMember = true;
                }

                if (totalPointsRedeemed >= 2000 && !loyaltyMember)
                {
                    var transactionLoyal = new Transaction
                    {
                        UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier),
                        DateOfTransaction = DateTime.Now,
                        AmountBeforeDiscount = 0,
                        PointsApplied = 1000,
                    };
                    _context.Add(transactionLoyal);
                    await context.SaveChangesAsync();
                    blnReturn = true;

                    // update user table
                    ApplicationUser userModel = await _userManager.FindByIdAsync(transactionLoyal.UserId);
                    //(User.Identity.GetUserId());

                    userModel.AvailablePoints += transactionLoyal.PointsApplied;
                    IdentityResult result = await _userManager.UpdateAsync(userModel);

                }
            }

            return blnReturn;
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
