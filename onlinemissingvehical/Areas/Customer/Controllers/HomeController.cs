using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlinemissingvehical.Data;
using onlinemissingvehical.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace onlinemissingvehical.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    var statusUpdates = await _context.StatusUpdates
                        .Include(s => s.MissingVehicle)
                        .ThenInclude(m => m.User) 
                        .ToListAsync();
                    return View(statusUpdates);
                }
                else
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var statusUpdates = await _context.StatusUpdates
                        .Include(s => s.MissingVehicle)
                        .ThenInclude(m => m.User)
                        .Where(s => s.MissingVehicle.UserId == userId)
                        .ToListAsync();
                    return View(statusUpdates);
                }
            }

            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
