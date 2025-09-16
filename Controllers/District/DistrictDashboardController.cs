using APATools.Context;
using Microsoft.AspNetCore.Mvc;

namespace APATools.Controllers.District
{
    public class DistrictDashboardController : Controller
    {
        private readonly ILogger<DistrictDashboardController> _logger;
        private readonly APAToolsContext _context;
        public DistrictDashboardController(ILogger<DistrictDashboardController> logger, APAToolsContext context)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == "7")
            {
                return View();
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
    }
}
