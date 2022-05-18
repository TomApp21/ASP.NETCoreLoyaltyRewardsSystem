using ASP.NETCoreLoyaltyRewardsSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP.NETCoreLoyaltyRewardsSystem.Controllers
{
    public class RewardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RewardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var allTransactions = await _context.Transactions.ToListAsync();


            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userTransactions = allTransactions.Where(x => x.UserId == userId).ToList();
            return View(userTransactions);


            return View();
        }
    }
}
