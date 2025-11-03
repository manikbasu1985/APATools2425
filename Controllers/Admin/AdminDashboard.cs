using APATools.Context;
using APATools.Models;
using APATools.Models.AdminModels;
using APATools.Models.ReportModels;
using APATools.OldContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace APATools.Controllers.Admin
{
    public class AdminDashboard : Controller
    {
        private readonly ILogger<AdminDashboard> _logger;
        private readonly APAToolsContext _context;
        private readonly GPIMSContext _gpimsContext;
        public AdminDashboard(ILogger<AdminDashboard> logger, APAToolsContext context, GPIMSContext gpimsContext)
        {
            _context = context;
            _logger = logger;
            _gpimsContext = gpimsContext;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == "1")
            {
                return View();
            }
            else
            {
                TempData["Failed"] = "Your are not allowed to Login";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> AssignDistrictofAPAfromPMU()
        {
            if(HttpContext.Session.GetString("isLoggedIn") == "Admin")
            {
                var data = await _context.Procedures.Sp_APA_List_Allowed_District_at_PMUAsync();
                ViewBag.Info = data;
                return View();
            }
            else
            {
                TempData["Failed"] = "User not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDistrictofAPAfromPMU(mst_LocationDistrict result)
        {
            if(result  != null) 
            {
                // Start a transaction to ensure atomicity (optional but recommended)
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var data = _context.A_APA_District_AllowChecks.Where(q => q.DistrictCode == result.DistrictCode).FirstOrDefault();
                    if (data == null)
                    {                        
                        _context.A_APA_District_AllowChecks.Add(new A_APA_District_AllowCheck
                        {
                            DistrictCode = result.DistrictCode,
                            AllowCheck = 1
                        });
                        await _context.Procedures.Sp_Edit_Remove_From_GP_for_APAAsync(result.DistrictCode);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        
                        TempData["Success"] = "Your District Added Successfully";
                    }
                    else
                    {
                        TempData["Failed"] = "Your District Added Already";
                    }
                }
                catch 
                {
                    // Rollback on error
                    await transaction.RollbackAsync();
                    TempData["Failed"] = "Your District Added Already";
                }
                return RedirectToAction("AssignDistrictofAPAfromPMU","AdminDashboard");
            }
            else
            {
                TempData["Failed"] = "Result value is null";
                return View();
            }
                
        }
        public JsonResult DistrictList()
        {
            var uncheckedDistricts = _context.mst_LocationDistricts
            .Where(district => !_context.A_APA_District_AllowChecks
                .Any(allowCheck => allowCheck.DistrictCode == district.DistrictCode)).ToList();
            /*var info = from loc in _context.mst_LocationDistricts
                       join dt in _context.A_APA_District_AllowChecks on loc.DistrictCode !equals dt.DistrictCode
                       select new
                       {
                           DistrictCode = loc.DistrictCode,
                           DistrictName = loc.DistrictName,
                       };*/
            /*var data = _context.mst_LocationDistricts.Select(q => new
            {
                DistrictCode = q.DistrictCode,
                DistrictName = q.DistrictName,
            });*/
            return Json(uncheckedDistricts);
        }
        public async Task<IActionResult> GetResults()
        {
            if (HttpContext.Session.GetString("UserRole") == "1")
            {
                var data = await _context.Procedures.Sp_APA_GetAllMC_TE_ResultsAsync();
                var info = data.OrderBy(q => q.DistrictName).ToList();
                return View(info);
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }

        public async Task<IActionResult> UpdateActiveStatus()
        {
            if (HttpContext.Session.GetString("UserRole") == "1")
            {
                var tableList = TableListProvider.GetAllowedTables();
                ViewBag.TableList = new SelectList(tableList);
                return View();
            }
            else
            {
                TempData["Failed"] = "Your are not allowed to Login";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateActiveStatus(UpdateActiveStatus result, long DistrictCode, string TableName)
        {
            var info = _context.mst_LocationDistricts.Where(q => q.DistrictCode == DistrictCode).FirstOrDefault();
            var data = _context.Procedures.Sp_Edit_Update_From_GP_for_APAAsync(result.TableName, result.DistrictCode);
            
            await _context.SaveChangesAsync();
            TempData["Success"] = "Data updated successfully of" + $"Updated {TableName} for District {info.DistrictName}"; ;
            return RedirectToAction("UpdateActiveStatus","AdminDashboard");
        }
        public JsonResult DistrictListforActivestatusCheck()
        {
            var data = _context.mst_LocationDistricts.Select(q=> new
            {
                DistrictCode = q.DistrictCode,
                DistrictName = q.DistrictName
            }).ToList();
            return Json(data);
        }
        public async Task<IActionResult> GetResultwithValue()
        {
            if (HttpContext.Session.GetString("UserRole") == "1" || HttpContext.Session.GetString("UserRole") == "4")
            {
                var data = await _context.Procedures.Sp_APA_GetAllMC_TE_Results_with_ValuesAsync();
                return View(data);
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
            
        }
    }
}
