using APATools.Context;
using APATools.Controllers.District;
using Microsoft.AspNetCore.Mvc;

namespace APATools.Controllers.Block
{
    public class BlockDashboardController : Controller
    {
        private readonly ILogger<BlockDashboardController> _logger;
        private readonly APAToolsContext _context;
        public BlockDashboardController(ILogger<BlockDashboardController> logger, APAToolsContext context)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == "9")
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
