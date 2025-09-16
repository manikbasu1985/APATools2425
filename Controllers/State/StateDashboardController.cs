using APATools.Context;
using Microsoft.AspNetCore.Mvc;

namespace APATools.Controllers.State
{
    public class StateDashboardController : Controller
    {
        private readonly ILogger<StateDashboardController> _logger;
        private readonly APAToolsContext _context;
        private readonly IWebHostEnvironment _environment;
        public StateDashboardController(ILogger<StateDashboardController> logger, APAToolsContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == "4")
            {
                return View();
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult Download(string filename, string gpcode)
        {
            var gpCode = Convert.ToInt32(gpcode);
            var loc = _context.view_alllocations.Where(q => q.GPCode == gpCode).FirstOrDefault();
            var location = loc.GPName + "_" + gpCode;
            if (string.IsNullOrEmpty(filename))
            {
                return Content("Filename is not provided.");
            }

            string filePath = Path.Combine(_environment.WebRootPath, "storages", filename);
            if (!System.IO.File.Exists(filePath))
            {
                return Content("File not found.");
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string exten = Path.GetExtension(filename);
            string filename_change = location + "_"+ DateTime.Now.ToString() + exten;
            //return File(fileBytes, "application/octet-stream", filename);
            return File(fileBytes, "application/octet-stream", filename_change);
        }
        //public 
        
    }
}
