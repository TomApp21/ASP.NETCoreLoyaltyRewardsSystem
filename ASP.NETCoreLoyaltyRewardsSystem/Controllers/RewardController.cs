using ASP.NETCoreLoyaltyRewardsSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP.NETCoreLoyaltyRewardsSystem.Controllers
{
    public class RewardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;


        public RewardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }




        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            return View(user);

        }


        public async Task<string> ReadPoints()
        {
            decimal rtnPoints = 0;

            // get user
            ApplicationUser userModel = await _userManager.FindByIdAsync(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            rtnPoints = userModel.AvailablePoints;

            if (rtnPoints > 1000)
            {
                rtnPoints = 100;
            }
            else
            {
                rtnPoints = (decimal)(rtnPoints / 1000) * 100;
            }

            int x = Convert.ToInt32(rtnPoints);


            return x.ToString();
        }


    }
}
