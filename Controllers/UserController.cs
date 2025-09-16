using APATools.Context;
using APATools.Models;
using APATools.OldContext;
using Microsoft.AspNetCore.Mvc;

namespace APATools.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly APAToolsContext _context;
        private readonly GPIMSContext _gpimsContext;
        public UserController(ILogger<UserController> logger, APAToolsContext context, GPIMSContext gpimsContext)
        {
            _context = context;
            _logger = logger;
            _gpimsContext = gpimsContext;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Sp_UserLoginResult result)
        {
            if (ModelState.IsValid)
            {
                var data = _gpimsContext.mst_UserMasters.Where(q => q.UserName.Equals(result.UserName) && q.UserPassword.Equals(result.UserPassword)).FirstOrDefault();
                var loc = _context.view_alllocations.Where(q => q.GPCode == data.AccessID);
                
                if (data != null && data.RoleCode.ToString() == "10")
                {
                    HttpContext.Session.SetString("isLoggedIn", "GPAdmin");
                    HttpContext.Session.SetString("UserInfo", data.UserName);
                    HttpContext.Session.SetString("UserRole", data.RoleCode.ToString());
                    HttpContext.Session.SetString("UserAccessID", data.AccessID.ToString());
                    return RedirectToAction("Index", "GPDashboard");
                }
                else if (data != null && data.RoleCode.ToString() == "1")
                {
                    HttpContext.Session.SetString("isLoggedIn", "Admin");
                    HttpContext.Session.SetString("UserInfo", data.UserName);
                    HttpContext.Session.SetString("UserRole", data.RoleCode.ToString());
                    HttpContext.Session.SetString("UserAccessID", data.AccessID.ToString());
                    return RedirectToAction("Index", "AdminDashboard");
                }
                else if (data != null && data.RoleCode.ToString() == "4")
                {
                    HttpContext.Session.SetString("isLoggedIn", "PMUAdmin");
                    HttpContext.Session.SetString("UserInfo", data.UserName);
                    HttpContext.Session.SetString("UserRole", data.RoleCode.ToString());
                    HttpContext.Session.SetString("UserAccessID", data.AccessID.ToString());
                    return RedirectToAction("Index", "StateDashboard");
                }
                else if (data != null && data.RoleCode.ToString() == "7")
                {
                    HttpContext.Session.SetString("isLoggedIn", "DistrictAdmin");
                    HttpContext.Session.SetString("UserInfo", data.UserName);
                    HttpContext.Session.SetString("UserRole", data.RoleCode.ToString());
                    HttpContext.Session.SetString("UserAccessID", data.AccessID.ToString());
                    return RedirectToAction("Index", "DistrictDashboard");
                }
                else if (data != null && data.RoleCode.ToString() == "9")
                {
                    HttpContext.Session.SetString("isLoggedIn", "BlockAdmin");
                    HttpContext.Session.SetString("UserInfo", data.UserName);
                    HttpContext.Session.SetString("UserRole", data.RoleCode.ToString());
                    HttpContext.Session.SetString("UserAccessID", data.AccessID.ToString());
                    return RedirectToAction("Index", "BlockDashboard");
                }
                else
                {
                    ModelState.AddModelError("Failure", "You are not a valid user.");
                    TempData["Failed"] = "No user found";
                    return RedirectToAction("Login", "User");
                }
            }
            else
            {
                ModelState.AddModelError("Failure", "User Name or Password are not valid");
                TempData["Failed"] = "No User Found++";
                return View();
            }
        }
        public IActionResult Logout()
        {
            TempData.Clear();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
    }
}
