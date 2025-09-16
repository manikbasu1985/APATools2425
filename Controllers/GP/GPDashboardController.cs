using APATools.Context;
using APATools.Models;
using APATools.OldContext;
using Microsoft.AspNetCore.Mvc;

namespace APATools.Controllers.GP
{
    public class GPDashboardController : Controller
    {
        private readonly ILogger<GPDashboardController> _logger;
        private readonly APAToolsContext _context;
        private readonly GPIMSContext _gpimsContext;
        private readonly IWebHostEnvironment _environment;
        public GPDashboardController(ILogger<GPDashboardController> logger, APAToolsContext context, GPIMSContext gPIMSContext, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
            _gpimsContext = gPIMSContext;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == "10")
            {
                return View();
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_MC_1()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_MC_1(A_APA_MC_1 result, IFormFile EvidenceofPlanApprovalResolution, IFormFile TotalMembersDeclaration_pdf, IFormFile TotalMembersDeclaration_excel)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 6;
            var data = _context.A_APA_MC_1s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && EvidenceofPlanApprovalResolution != null && TotalMembersDeclaration_excel != null && TotalMembersDeclaration_pdf != null)
            {
                if (EvidenceofPlanApprovalResolution.Length > MaxContentLength && TotalMembersDeclaration_pdf.Length > MaxContentLength && TotalMembersDeclaration_excel.Length > MaxContentLength)
                {
                    ModelState.AddModelError("EvidenceofPlanApprovalResolution & TotalMembersDeclaration", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();

                    var allowedExtensions = new[] { ".pdf", ".jpg", ".png" };

                    var fileNameEvidenceofPlanApprovalResolution = gpCode.ToString() + "-" + "epar" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var fileNameTotalMembersDeclaration_pdf = gpCode.ToString() + "-" + "tmd" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var fileNameTotalMembersDeclaration_excel = gpCode.ToString() + "-" + "tmd_excel" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    var extensionEvidenceofPlanApprovalResolution = Path.GetExtension(EvidenceofPlanApprovalResolution.FileName);
                    var extensionTotalMembersDeclaration_pdf = Path.GetExtension(TotalMembersDeclaration_pdf.FileName);
                    var extensionTotalMembersDeclaration_excel = Path.GetExtension(TotalMembersDeclaration_excel.FileName);
                    
                    if(extensionEvidenceofPlanApprovalResolution == ".pdf" && extensionTotalMembersDeclaration_pdf == ".pdf" && extensionTotalMembersDeclaration_excel == ".xlsx" || extensionTotalMembersDeclaration_excel == ".xls")
                    {
                        string uploadPathTotalMembersDeclaration_pdf = Path.Combine(_environment.WebRootPath, "storages/apa_mc_1/TMD");

                        string filePathTotalMembersDeclaration_pdf = Path.Combine(uploadPathTotalMembersDeclaration_pdf,
                            fileNameTotalMembersDeclaration_pdf + extensionTotalMembersDeclaration_pdf);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathTotalMembersDeclaration_pdf))
                        {
                            // The file is saved in a buffer before being processed
                            await TotalMembersDeclaration_pdf.CopyToAsync(stream);
                        }
                        string uploadPathTotalMembersDeclaration_excel = Path.Combine(_environment.WebRootPath, "storages/apa_mc_1/TMD_EXCEL");

                        string filePathTotalMembersDeclaration_excel = Path.Combine(uploadPathTotalMembersDeclaration_excel,
                            fileNameTotalMembersDeclaration_excel + extensionTotalMembersDeclaration_excel);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathTotalMembersDeclaration_excel))
                        {
                            // The file is saved in a buffer before being processed
                            await TotalMembersDeclaration_excel.CopyToAsync(stream);
                        }
                        // Define the upload folder
                        string uploadPathEvidenceofPlanApprovalResolution = Path.Combine(_environment.WebRootPath, "storages/apa_mc_1/EPAR");
                        // Generate the file path
                        string filePathEvidenceofPlanApprovalResolution = Path.Combine(uploadPathEvidenceofPlanApprovalResolution,
                            fileNameEvidenceofPlanApprovalResolution + extensionEvidenceofPlanApprovalResolution);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathEvidenceofPlanApprovalResolution))
                        {
                            // The file is saved in a buffer before being processed
                            await EvidenceofPlanApprovalResolution.CopyToAsync(stream);
                        }
                        try
                        {
                            //Data Upload in Database
                            _context.A_APA_MC_1s.Add(new A_APA_MC_1()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                EvidenceofPlanApprovalResolution = fileNameEvidenceofPlanApprovalResolution + extensionEvidenceofPlanApprovalResolution,
                                EvidenceofPlanApprovalResolution_path = filePathEvidenceofPlanApprovalResolution,
                                TotalMembersDeclaration_excel = fileNameTotalMembersDeclaration_excel + extensionTotalMembersDeclaration_excel,
                                TotalMembersDeclaration_excel_path = filePathTotalMembersDeclaration_excel,
                                TotalMembersDeclaration_pdf = fileNameTotalMembersDeclaration_pdf + extensionTotalMembersDeclaration_pdf,
                                TotalMembersDeclaration_pdf_path = filePathTotalMembersDeclaration_pdf,
                                TotalNoofMember = result.TotalNoofMember,
                                NoofMemberAttended = result.NoofMemberAttended,
                                PlanApprovalDate = result.PlanApprovalDate,
                                SingleAgendaMeeting = result.SingleAgendaMeeting,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your Report Uploaded Successfully!";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_MC_1_Report", "GPReport");
                    }
                    else 
                    {
                        ModelState.AddModelError("EvidenceofPlanApprovalResolution & TotalMembersDeclaration", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_MC_1", "GPDashboard");
            }
        }
        public IActionResult APA_MC_2()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_MC_2(A_APA_MC_2 result, IFormFile DeclarationStatusPhysicalCompletedActivities, IFormFile DeclarationPlan_Implementation,
            IFormFile EvidenceofCompletedActivity)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            var data = _context.A_APA_MC_2s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && DeclarationStatusPhysicalCompletedActivities != null && DeclarationPlan_Implementation != null &&
                EvidenceofCompletedActivity != null && DeclarationStatusPhysicalCompletedActivities.Length > 0 && DeclarationPlan_Implementation.Length > 0
                && EvidenceofCompletedActivity.Length > 0)
            {
                var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                int MaxContentLength = 1024 * 1024 * 11;
                int MaxContentLength1 = 1024 * 1024 * 26;
                var allowedExtensions = new[] { ".pdf", ".jpg", ".png" };

                var fileNameDeclarationStatusPhysicalCompletedActivities = gpCode.ToString() + "-" + "dspca" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var fileNameDeclarationPlan_Implementation = gpCode.ToString() + "-" + "dpi" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var fileNameEvidenceofCompletedActivity = gpCode.ToString() + "-" + "eoca" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                var extensionDeclarationStatusPhysicalCompletedActivities = Path.GetExtension(DeclarationStatusPhysicalCompletedActivities.FileName);
                var extensionDeclarationPlan_Implementation = Path.GetExtension(DeclarationPlan_Implementation.FileName);
                var extensionEvidenceofCompletedActivity = Path.GetExtension(EvidenceofCompletedActivity.FileName);

                if (DeclarationStatusPhysicalCompletedActivities.Length > MaxContentLength && DeclarationPlan_Implementation.Length > MaxContentLength1 &&
                    EvidenceofCompletedActivity.Length > MaxContentLength)
                {
                    ModelState.AddModelError("DeclarationStatusPhysicalCompletedActivities, DeclarationPlan_Implementation & EvidenceofCompletedActivity", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    if (extensionDeclarationPlan_Implementation == ".pdf" && extensionDeclarationStatusPhysicalCompletedActivities == ".xlsx" || extensionDeclarationStatusPhysicalCompletedActivities == ".xls"
                        && extensionEvidenceofCompletedActivity == ".pdf")
                    {
                        string uploadPathDeclarationStatusPhysicalCompletedActivities = Path.Combine(_environment.WebRootPath, "storages/apa_mc_2/DSPCA");
                        string filePathDeclarationStatusPhysicalCompletedActivities = Path.Combine(uploadPathDeclarationStatusPhysicalCompletedActivities,
                            fileNameDeclarationStatusPhysicalCompletedActivities + extensionDeclarationStatusPhysicalCompletedActivities);
                        using (var stream = System.IO.File.Create(filePathDeclarationStatusPhysicalCompletedActivities))
                        {
                            await DeclarationStatusPhysicalCompletedActivities.CopyToAsync(stream);
                        }
                        string uploadPathDeclarationPlan_Implementation = Path.Combine(_environment.WebRootPath, "storages/apa_mc_2/DPI");
                        string filePathDeclarationPlan_Implementation = Path.Combine(uploadPathDeclarationPlan_Implementation,
                            fileNameDeclarationPlan_Implementation + extensionDeclarationPlan_Implementation);
                        using (var stream = System.IO.File.Create(filePathDeclarationPlan_Implementation))
                        {
                            await DeclarationPlan_Implementation.CopyToAsync(stream);
                        }
                        string uploadPathEvidenceofCompletedActivity = Path.Combine(_environment.WebRootPath, "storages/apa_mc_2/EOCA");
                        string filePathEvidenceofCompletedActivity = Path.Combine(uploadPathEvidenceofCompletedActivity,
                            fileNameEvidenceofCompletedActivity + extensionEvidenceofCompletedActivity);
                        using (var stream = System.IO.File.Create(filePathEvidenceofCompletedActivity))
                        {
                            await EvidenceofCompletedActivity.CopyToAsync(stream);
                        }
                        try
                        {
                            //Data Upload in Database
                            _context.A_APA_MC_2s.Add(new A_APA_MC_2()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                NoofIssuedWorkOrder = result.NoofIssuedWorkOrder,
                                NoofPhysicallyCompletedActivities = result.NoofPhysicallyCompletedActivities,
                                DeclarationPlan_Implementation = fileNameDeclarationPlan_Implementation + extensionDeclarationPlan_Implementation,
                                DeclarationPlan_Implementation_Path = filePathDeclarationPlan_Implementation,

                                DeclarationStatusPhysicalCompletedActivities = fileNameDeclarationStatusPhysicalCompletedActivities + extensionDeclarationStatusPhysicalCompletedActivities,
                                DeclarationStatusPhysicalCompletedActivities_Path = filePathDeclarationStatusPhysicalCompletedActivities,

                                EvidenceofCompletedActivity = fileNameEvidenceofCompletedActivity + extensionEvidenceofCompletedActivity,
                                EvidenceofCompletedActivity_Path = filePathEvidenceofCompletedActivity,

                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your Report Uploaded Successfully!";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("DeclarationStatusPhysicalCompletedActivities, DeclarationPlan_Implementation & EvidenceofCompletedActivity", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }

                    return RedirectToAction("APA_MC_2_Report", "GPReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_MC_2", "GPDashboard");
            }
        }
        public JsonResult getFinacialYearforMC5()
        {
            var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Take(4).ToList();
            var data = fin.Skip(1).Select(q => new
            {
                FYCode = q.FYCode,
                FYName = q.FYName
            });
            return Json(data);
        }

        public IActionResult APA_MC_6()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                var oldAPA = _gpimsContext.A_APA_MC_6_olds.Where(q => q.GPCode.Equals(accessID.GPCode)).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                    if (oldAPA != null)
                    {
                        ViewBag.TotalTaxinINR_2324 = oldAPA.TotalTaxinINR_2324;
                        ViewBag.TotalNonTaxinINR_2324 = oldAPA.TotalNonTaxinINR_2324;
                        ViewBag.TotalOSRinINR_2324 = oldAPA.TotalOSRinINR_2324;
                        ViewBag.OSRDeductionAmountinINR_2324 = oldAPA.OSRDeductionAmountinINR_2324;
                    }
                    else
                    {
                        TempData["Failed"] = "No data found in old APA";
                    }
                }
                else
                {
                    TempData["Failed"] = "GP Code Not Found";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_MC_6(A_APA_MC_6 result, IFormFile OSRDataUpload,
            IFormFile CCERUpload_2024_2025)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 6;
            var data = _context.A_APA_MC_6s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && CCERUpload_2024_2025 != null && OSRDataUpload != null)
            {
                if (CCERUpload_2024_2025.Length > MaxContentLength && OSRDataUpload.Length > MaxContentLength)
                {
                    ModelState.AddModelError("CCERUpload_2023_2024 & CCERUpload_2024_2025", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return RedirectToAction("APA_MC_6", "GPDashboard");
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions = new[] { ".pdf", ".jpg", ".png" };
                    var allowedExtensions_osr = new[] { ".xlsx", ".xls" };
                    var fileNameCCERUpload_2024_2025 = gpCode.ToString() + "-" + "ccer2425" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var fileNameOSRDataUpload = gpCode.ToString() + "-" + "osr2425" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionCCERUpload_2024_2025 = Path.GetExtension(CCERUpload_2024_2025.FileName);
                    var extensionOSRDataUpload = Path.GetExtension(OSRDataUpload.FileName);
                    if (extensionCCERUpload_2024_2025 == ".pdf" && extensionOSRDataUpload == ".xlsx" || extensionOSRDataUpload == ".xls")
                    {

                        string uploadPathCCERUpload_2024_2025 = Path.Combine(_environment.WebRootPath, "storages/apa_mc_6/CCEROSR/2425");
                        string filePathCCERUpload_2024_2025 = Path.Combine(uploadPathCCERUpload_2024_2025,
                            fileNameCCERUpload_2024_2025 + extensionCCERUpload_2024_2025);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathCCERUpload_2024_2025))
                        {
                            // The file is saved in a buffer before being processed
                            await CCERUpload_2024_2025.CopyToAsync(stream);
                        }
                        string uploadPathOSRDataUpload = Path.Combine(_environment.WebRootPath, "storages/apa_mc_6/OSR");
                        string filePathOSRDataUpload = Path.Combine(uploadPathOSRDataUpload,fileNameOSRDataUpload + extensionOSRDataUpload);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathOSRDataUpload))
                        {
                            // The file is saved in a buffer before being processed
                            await OSRDataUpload.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_MC_6s.Add(new A_APA_MC_6()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                CCERUpload_2024_2025 = fileNameCCERUpload_2024_2025 + extensionCCERUpload_2024_2025,
                                CCERUpload_2024_2025_path = filePathCCERUpload_2024_2025,
                                OSRDataUpload = fileNameOSRDataUpload + extensionOSRDataUpload,
                                OSRDataUpload_path = filePathOSRDataUpload,
                                
                                TotalTaxinINR_2324 = result.TotalTaxinINR_2324,

                                TotalTaxinINR_2425 = result.TotalTaxinINR_2425,

                                TotalNonTaxinINR_2324 = result.TotalNonTaxinINR_2324,
                                TotalNonTaxinINR_2425 = result.TotalNonTaxinINR_2425,

                                TotalOSRinINR_2324 = result.TotalOSRinINR_2324,
                                //TotalOSRinINR_2425 = result.TotalOSRinINR_2425,
                                TotalOSRinINR_2425 = result.TotalTaxinINR_2425 + result.TotalNonTaxinINR_2425,

                                OSRDeductionAmountinINR_2324 = result.OSRDeductionAmountinINR_2324,
                                OSRDeductionAmountinINR_2425 = result.OSRDeductionAmountinINR_2425,

                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_MC_6_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_MC_6", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("CCERUpload_2023_2024 & CCERUpload_2024_2025 & OSRDataUpload", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_MC_6", "GPDashboard");
            }
        }
        public IActionResult APA_TE_1()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_1(A_APA_TE_1 result, IFormFile PlanActivitiesUploadForPDF, IFormFile PlanActivitiesUploadForExcel)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 6;
            var data = _context.A_APA_TE_1s.Where(q => q.GPCode == gpCode).FirstOrDefault();

            if (data == null && PlanActivitiesUploadForPDF != null && PlanActivitiesUploadForExcel != null)
            {
                if (PlanActivitiesUploadForPDF.Length > MaxContentLength && PlanActivitiesUploadForExcel.Length > MaxContentLength)
                {
                    ModelState.AddModelError("PlanActivitiesUploadForPDF & PlanActivitiesUploadForExcel", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var allowedExtensions_xls = new[] { ".xlsx", ".xls" };
                    var fileNamePlanActivitiesUploadForPDF = gpCode.ToString() + "-" + "anex4pdf" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var fileNamePlanActivitiesUploadForExcel = gpCode.ToString() + "-" + "anex4xls" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    var extensionPlanActivitiesUploadForPDF = Path.GetExtension(PlanActivitiesUploadForPDF.FileName);
                    var extensionPlanActivitiesUploadForExcel = Path.GetExtension(PlanActivitiesUploadForExcel.FileName);

                    if (extensionPlanActivitiesUploadForPDF == ".pdf" && extensionPlanActivitiesUploadForExcel == ".xlsx" || extensionPlanActivitiesUploadForExcel == ".xls")
                    {
                        string uploadPathPlanActivitiesUploadForPDF = Path.Combine(_environment.WebRootPath, "storages/apa_te_1/ANEXPDF");
                        string filePathPlanActivitiesUploadForPDF = Path.Combine(uploadPathPlanActivitiesUploadForPDF,
                            fileNamePlanActivitiesUploadForPDF + extensionPlanActivitiesUploadForPDF);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathPlanActivitiesUploadForPDF))
                        {
                            // The file is saved in a buffer before being processed
                            await PlanActivitiesUploadForPDF.CopyToAsync(stream);
                        }
                        string uploadPathPlanActivitiesUploadForExcel = Path.Combine(_environment.WebRootPath, "storages/apa_te_1/ANEXEXCEL");
                        string filePathPlanActivitiesUploadForExcel = Path.Combine(uploadPathPlanActivitiesUploadForExcel,
                            fileNamePlanActivitiesUploadForExcel + extensionPlanActivitiesUploadForExcel);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathPlanActivitiesUploadForExcel))
                        {
                            // The file is saved in a buffer before being processed
                            await PlanActivitiesUploadForExcel.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_1s.Add(new A_APA_TE_1()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                TotalNoofPlanActivities = result.TotalNoofPlanActivities,
                                TotalNoofPlanUnderSankalpa = result.TotalNoofPlanUnderSankalpa,
                                PlanActivitiesUploadForPDF = fileNamePlanActivitiesUploadForPDF + extensionPlanActivitiesUploadForPDF,
                                PlanActivitiesUploadForPDF_Path = filePathPlanActivitiesUploadForPDF,
                                PlanActivitiesUploadForExcel = fileNamePlanActivitiesUploadForExcel + extensionPlanActivitiesUploadForExcel,
                                PlanActivitiesUploadForExcel_Path = filePathPlanActivitiesUploadForExcel,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_1_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_1", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("PlanActivitiesUploadForPDF & PlanActivitiesUploadForEXCEL", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return RedirectToAction("APA_TE_1", "GPDashboard");
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_1", "GPDashboard");
            }
        }
        public IActionResult APA_TE_2()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_2(A_APA_TE_2 result, IFormFile Form36Doc)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_2s.Where(q => q.GPCode == gpCode).FirstOrDefault();

            if (data == null && Form36Doc != null)
            {
                if (Form36Doc.Length > MaxContentLength)
                {
                    ModelState.AddModelError("Form36Doc", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameForm36Doc = gpCode.ToString() + "-" + "form36" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    var extensionForm36Doc = Path.GetExtension(Form36Doc.FileName);
                    if (extensionForm36Doc == ".xlsx" || extensionForm36Doc == ".xls")
                    {
                        string uploadPathForm36Doc = Path.Combine(_environment.WebRootPath, "storages/apa_te_2/FORM36");
                        string filePathForm36Doc = Path.Combine(uploadPathForm36Doc,
                            fileNameForm36Doc + extensionForm36Doc);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathForm36Doc))
                        {
                            // The file is saved in a buffer before being processed
                            await Form36Doc.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_2s.Add(new A_APA_TE_2()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                TotalBudgetfor2526FY = result.TotalBudgetfor2526FY,
                                TotalBudgetmore3_5Lakh = result.TotalBudgetmore3_5Lakh,
                                Form36Doc = fileNameForm36Doc + extensionForm36Doc,
                                Form36Doc_Path = filePathForm36Doc,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_2_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_2", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("Form36Doc", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_2", "GPDashboard");
            }
        }
        public IActionResult APA_TE_3()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_3(A_APA_TE_3 result, IFormFile MeetingResolutionUpload, IFormFile GPDeclarationUpload)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength_25 = 1024 * 1024 * 26;
            int MaxContentLength_11 = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_3s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && MeetingResolutionUpload != null && GPDeclarationUpload != null)
            {
                if (MeetingResolutionUpload.Length > MaxContentLength_25 & GPDeclarationUpload.Length > MaxContentLength_11)
                {
                    ModelState.AddModelError("MeetingResolutionUpload & GPDeclarationUpload", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameMeetingResolutionUpload = gpCode.ToString() + "-" + "mru" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var fileNameGPDeclarationUpload = gpCode.ToString() + "-" + "gdu" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionMeetingResolutionUpload = Path.GetExtension(MeetingResolutionUpload.FileName);
                    var extensionGPDeclarationUpload = Path.GetExtension(GPDeclarationUpload.FileName);
                    if (extensionMeetingResolutionUpload == ".pdf" && extensionGPDeclarationUpload == ".pdf")
                    {
                        string uploadPathMeetingResolutionUpload = Path.Combine(_environment.WebRootPath, "storages/apa_te_3/MRU");
                        string filePathMeetingResolutionUpload = Path.Combine(uploadPathMeetingResolutionUpload,
                            fileNameMeetingResolutionUpload + extensionMeetingResolutionUpload);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathMeetingResolutionUpload))
                        {
                            // The file is saved in a buffer before being processed
                            await MeetingResolutionUpload.CopyToAsync(stream);
                        }
                        string uploadPathGPDeclarationUpload = Path.Combine(_environment.WebRootPath, "storages/apa_te_3/GDU");
                        string filePathGPDeclarationUpload = Path.Combine(uploadPathGPDeclarationUpload,
                            fileNameGPDeclarationUpload + extensionGPDeclarationUpload);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathGPDeclarationUpload))
                        {
                            // The file is saved in a buffer before being processed
                            await GPDeclarationUpload.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_3s.Add(new A_APA_TE_3()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                TotalNoofMeetingsConducted = result.TotalNoofMeetingsConducted,
                                MeetingResolutionUpload = fileNameMeetingResolutionUpload + extensionMeetingResolutionUpload,
                                MeetingResolutionUpload_path = filePathMeetingResolutionUpload,
                                GPDeclarationUpload = fileNameGPDeclarationUpload + extensionGPDeclarationUpload,
                                GPDeclarationUpload_path = filePathGPDeclarationUpload,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_3_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_3", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("MeetingResolutionUpload & GPDeclarationUpload", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return RedirectToAction("APA_TE_3", "GPDashboard");
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_3", "GPDashboard");
            }
        }
        public IActionResult APA_TE_4()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_4(A_APA_TE_4 result, IFormFile GramSavaMeetingResolution, IFormFile DeclarationofAnnex6)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_4s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && GramSavaMeetingResolution != null && DeclarationofAnnex6 != null)
            {
                if (GramSavaMeetingResolution.Length > MaxContentLength && DeclarationofAnnex6.Length > MaxContentLength)
                {
                    ModelState.AddModelError("GramSavaMeetingResolution & DeclarationofAnnex6", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameGramSavaMeetingResolution = gpCode.ToString() + "-" + "gsmr" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var fileNameDeclarationofAnnex6 = gpCode.ToString() + "-" + "doa6" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionGramSavaMeetingResolution = Path.GetExtension(GramSavaMeetingResolution.FileName);
                    var extensionDeclarationofAnnex6 = Path.GetExtension(DeclarationofAnnex6.FileName);
                    if (extensionGramSavaMeetingResolution == ".pdf" && extensionDeclarationofAnnex6 == ".pdf")
                    {
                        string uploadPathGramSavaMeetingResolution = Path.Combine(_environment.WebRootPath, "storages/apa_te_4/GSMR");
                        string filePathGramSavaMeetingResolution = Path.Combine(uploadPathGramSavaMeetingResolution,
                            fileNameGramSavaMeetingResolution + extensionGramSavaMeetingResolution);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathGramSavaMeetingResolution))
                        {
                            // The file is saved in a buffer before being processed
                            await GramSavaMeetingResolution.CopyToAsync(stream);
                        }
                        string uploadPathDeclarationofAnnex6 = Path.Combine(_environment.WebRootPath, "storages/apa_te_4/DOA6");
                        string filePathDeclarationofAnnex6 = Path.Combine(uploadPathDeclarationofAnnex6,
                            fileNameDeclarationofAnnex6 + extensionDeclarationofAnnex6);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathDeclarationofAnnex6))
                        {
                            // The file is saved in a buffer before being processed
                            await DeclarationofAnnex6.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_4s.Add(new A_APA_TE_4()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                TotalnoofActivitiesofGramSabha = result.TotalnoofActivitiesofGramSabha,
                                TotalnoofActivitiesRecomendedByGramSabha = result.TotalnoofActivitiesRecomendedByGramSabha,
                                GramSavaMeetingResolution = fileNameGramSavaMeetingResolution + extensionGramSavaMeetingResolution,
                                GramSavaMeetingResolution_Path = filePathGramSavaMeetingResolution,
                                DeclarationofAnnex6 = fileNameDeclarationofAnnex6 + extensionDeclarationofAnnex6,
                                DeclarationofAnnex6_path = filePathDeclarationofAnnex6,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_4_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_4", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("GramSavaMeetingResolution & DeclarationofAnnex6", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_4", "GPDashboard");
            }
        }
        public IActionResult APA_TE_5()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_5(A_APA_TE_5 result, IFormFile Activity_1_DPR, IFormFile Activity_2_DPR, IFormFile Activity_3_DPR,
            IFormFile Activity_4_DPR, IFormFile Activity_5_DPR, IFormFile CompletionCertificate, IFormFile EntireWOHightingAll)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_5s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && Activity_1_DPR != null && Activity_2_DPR != null && Activity_3_DPR != null && Activity_4_DPR != null &&
                Activity_5_DPR != null && CompletionCertificate != null && EntireWOHightingAll != null)
            {
                if (Activity_1_DPR.Length > MaxContentLength && Activity_2_DPR.Length > MaxContentLength &&
                    Activity_3_DPR.Length > MaxContentLength && Activity_4_DPR.Length > MaxContentLength && Activity_5_DPR.Length > MaxContentLength
                    && EntireWOHightingAll.Length > MaxContentLength && CompletionCertificate.Length > MaxContentLength)
                {
                    ModelState.AddModelError("DPRs & Annex-II, Completion Certificate", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameActivity_1_DPR = gpCode.ToString() + "-" + "dpr_1" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_1_DPR = Path.GetExtension(Activity_1_DPR.FileName);
                    var fileNameActivity_2_DPR = gpCode.ToString() + "-" + "dpr_2" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_2_DPR = Path.GetExtension(Activity_2_DPR.FileName);
                    var fileNameActivity_3_DPR = gpCode.ToString() + "-" + "dpr_3" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_3_DPR = Path.GetExtension(Activity_3_DPR.FileName);
                    var fileNameActivity_4_DPR = gpCode.ToString() + "-" + "dpr_4" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_4_DPR = Path.GetExtension(Activity_4_DPR.FileName);
                    var fileNameActivity_5_DPR = gpCode.ToString() + "-" + "dpr_5" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_5_DPR = Path.GetExtension(Activity_5_DPR.FileName);
                    var fileNameEntireWOHightingAll = gpCode.ToString() + "-" + "wohall" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionEntireWOHightingAll = Path.GetExtension(EntireWOHightingAll.FileName);
                    var fileNameCompletionCertificate = gpCode.ToString() + "-" + "cc" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionCompletionCertificate = Path.GetExtension(CompletionCertificate.FileName);
                    if (extensionActivity_1_DPR == ".pdf" && extensionActivity_2_DPR == ".pdf" && extensionActivity_3_DPR == ".pdf"
                        && extensionActivity_4_DPR == ".pdf" && extensionActivity_5_DPR == ".pdf" && extensionEntireWOHightingAll == ".xlsx"
                        && extensionCompletionCertificate == ".pdf" || extensionEntireWOHightingAll == ".xls")
                    {
                        string uploadPathActivity_1_DPR = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/DPR_1");
                        string filePathActivity_1_DPR = Path.Combine(uploadPathActivity_1_DPR,
                            fileNameActivity_1_DPR + extensionActivity_1_DPR);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_1_DPR))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_1_DPR.CopyToAsync(stream);
                        }
                        string uploadPathActivity_2_DPR = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/DPR_2");
                        string filePathActivity_2_DPR = Path.Combine(uploadPathActivity_2_DPR,
                            fileNameActivity_2_DPR + extensionActivity_2_DPR);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_2_DPR))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_2_DPR.CopyToAsync(stream);
                        }
                        string uploadPathActivity_3_DPR = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/DPR_3");
                        string filePathActivity_3_DPR = Path.Combine(uploadPathActivity_3_DPR,
                            fileNameActivity_3_DPR + extensionActivity_3_DPR);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_3_DPR))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_3_DPR.CopyToAsync(stream);
                        }
                        string uploadPathActivity_4_DPR = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/DPR_4");
                        string filePathActivity_4_DPR = Path.Combine(uploadPathActivity_4_DPR,
                            fileNameActivity_4_DPR + extensionActivity_4_DPR);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_4_DPR))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_4_DPR.CopyToAsync(stream);
                        }
                        string uploadPathActivity_5_DPR = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/DPR_5");
                        string filePathActivity_5_DPR = Path.Combine(uploadPathActivity_5_DPR,
                            fileNameActivity_5_DPR + extensionActivity_5_DPR);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_5_DPR))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_5_DPR.CopyToAsync(stream);
                        }
                        string uploadPathCompletionCertificate = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/CC");
                        string filePathCompletionCertificate = Path.Combine(uploadPathCompletionCertificate,
                            fileNameCompletionCertificate + extensionCompletionCertificate);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathCompletionCertificate))
                        {
                            // The file is saved in a buffer before being processed
                            await CompletionCertificate.CopyToAsync(stream);
                        }
                        string uploadPathEntireWOHightingAll = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/WOHA");
                        string filePathEntireWOHightingAll = Path.Combine(uploadPathEntireWOHightingAll,
                            fileNameEntireWOHightingAll + extensionEntireWOHightingAll);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathEntireWOHightingAll))
                        {
                            // The file is saved in a buffer before being processed
                            await EntireWOHightingAll.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_5s.Add(new A_APA_TE_5
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                ActivityName_1 = result.ActivityName_1,
                                ActivityName_2 = result.ActivityName_2,
                                ActivityName_3 = result.ActivityName_3,
                                ActivityName_4 = result.ActivityName_4,
                                ActivityName_5 = result.ActivityName_5,
                                ActivityValue_1 = result.ActivityValue_1,
                                ActivityValue_2 = result.ActivityValue_2,
                                ActivityValue_3 = result.ActivityValue_3,
                                ActivityValue_4 = result.ActivityValue_4,
                                ActivityValue_5 = result.ActivityValue_5,

                                FundofActivity_1 = result.FundofActivity_1,
                                FundofActivity_2 = result.FundofActivity_2,
                                FundofActivity_3 = result.FundofActivity_3,
                                FundofActivity_4 = result.FundofActivity_4,
                                FundofActivity_5 = result.FundofActivity_5,

                                Activity_1_DPR = fileNameActivity_1_DPR + extensionActivity_1_DPR,
                                Activity_1_DPR_Path = filePathActivity_1_DPR,
                                Activity_2_DPR = fileNameActivity_2_DPR + extensionActivity_2_DPR,
                                Activity_2_DPR_Path = filePathActivity_2_DPR,
                                Activity_3_DPR = fileNameActivity_3_DPR + extensionActivity_3_DPR,
                                Activity_3_DPR_Path = filePathActivity_3_DPR,
                                Activity_4_DPR = fileNameActivity_4_DPR + extensionActivity_4_DPR,
                                Activity_4_DPR_Path = filePathActivity_4_DPR,
                                Activity_5_DPR = fileNameActivity_5_DPR + extensionActivity_5_DPR,
                                Activity_5_DPR_Path = filePathActivity_5_DPR,
                                EntireWOHightingAll = fileNameEntireWOHightingAll + extensionEntireWOHightingAll,
                                EntireWOHightingAll_Path = filePathEntireWOHightingAll,
                                CompletionCertificate = fileNameCompletionCertificate + extensionCompletionCertificate,
                                CompletionCertificate_Path = filePathCompletionCertificate,
                                ActiveStatus = 1,
                                User_ID = HttpContext.Session.GetString("UserInfo").ToString(),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_5_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_5", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("GramSavaMeetingResolution & DeclarationofAnnex6", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_5", "GPDashboard");
            }
        }
        public IActionResult APA_TE_6()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_6(A_APA_TE_6 result, IFormFile Activity_1_MT, IFormFile Activity_2_MT, IFormFile Activity_3_MT,
            IFormFile Activity_4_MT, IFormFile Activity_5_MT, IFormFile CompletionCertificate_MT, IFormFile EntireWOHightingAll_MT)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);

            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_6s.Where(q => q.GPCode == gpCode).FirstOrDefault();


            if (data == null && Activity_1_MT != null && Activity_2_MT != null && Activity_3_MT != null && Activity_4_MT != null &&
                Activity_5_MT != null && CompletionCertificate_MT != null && EntireWOHightingAll_MT != null)
            {
                if (Activity_1_MT.Length > MaxContentLength && Activity_2_MT.Length > MaxContentLength &&
                    Activity_3_MT.Length > MaxContentLength && Activity_4_MT.Length > MaxContentLength && Activity_5_MT.Length > MaxContentLength
                    && EntireWOHightingAll_MT.Length > MaxContentLength && CompletionCertificate_MT.Length > MaxContentLength)
                {
                    ModelState.AddModelError("Material Testing & Annex, Completion Certificate", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameActivity_1_MT = gpCode.ToString() + "-" + "mt_1" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_1_MT = Path.GetExtension(Activity_1_MT.FileName);
                    var fileNameActivity_2_MT = gpCode.ToString() + "-" + "mt_2" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_2_MT = Path.GetExtension(Activity_2_MT.FileName);
                    var fileNameActivity_3_MT = gpCode.ToString() + "-" + "mt_3" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_3_MT = Path.GetExtension(Activity_3_MT.FileName);
                    var fileNameActivity_4_MT = gpCode.ToString() + "-" + "mt_4" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_4_MT = Path.GetExtension(Activity_4_MT.FileName);
                    var fileNameActivity_5_MT = gpCode.ToString() + "-" + "mt_5" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_5_MT = Path.GetExtension(Activity_5_MT.FileName);
                    var fileNameEntireWOHightingAll_MT = gpCode.ToString() + "-" + "wohallmt" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionEntireWOHightingAll_MT = Path.GetExtension(EntireWOHightingAll_MT.FileName);
                    var fileNameCompletionCertificate_MT = gpCode.ToString() + "-" + "ccmt" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionCompletionCertificate_MT = Path.GetExtension(CompletionCertificate_MT.FileName);
                    if (extensionActivity_1_MT == ".pdf" && extensionActivity_2_MT == ".pdf" && extensionActivity_3_MT == ".pdf"
                        && extensionActivity_4_MT == ".pdf" && extensionActivity_5_MT == ".pdf" && extensionEntireWOHightingAll_MT == ".xlsx" || extensionEntireWOHightingAll_MT == ".xls"
                        && extensionCompletionCertificate_MT == ".pdf")
                    {
                        string uploadPathActivity_1_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/MT_1");
                        string filePathActivity_1_MT = Path.Combine(uploadPathActivity_1_MT,
                            fileNameActivity_1_MT + extensionActivity_1_MT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_1_MT))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_1_MT.CopyToAsync(stream);
                        }
                        string uploadPathActivity_2_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/MT_2");
                        string filePathActivity_2_MT = Path.Combine(uploadPathActivity_2_MT,
                            fileNameActivity_2_MT + extensionActivity_2_MT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_2_MT))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_2_MT.CopyToAsync(stream);
                        }
                        string uploadPathActivity_3_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/MT_3");
                        string filePathActivity_3_MT = Path.Combine(uploadPathActivity_3_MT,
                            fileNameActivity_3_MT + extensionActivity_3_MT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_3_MT))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_3_MT.CopyToAsync(stream);
                        }
                        string uploadPathActivity_4_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/MT_4");
                        string filePathActivity_4_MT = Path.Combine(uploadPathActivity_4_MT,
                            fileNameActivity_4_MT + extensionActivity_4_MT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_4_MT))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_4_MT.CopyToAsync(stream);
                        }
                        string uploadPathActivity_5_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/MT_5");
                        string filePathActivity_5_MT = Path.Combine(uploadPathActivity_5_MT,
                            fileNameActivity_5_MT + extensionActivity_5_MT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_5_MT))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_5_MT.CopyToAsync(stream);
                        }
                        string uploadPathCompletionCertificate_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/CCMT");
                        string filePathCompletionCertificate_MT = Path.Combine(uploadPathCompletionCertificate_MT,
                            fileNameCompletionCertificate_MT + extensionCompletionCertificate_MT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathCompletionCertificate_MT))
                        {
                            // The file is saved in a buffer before being processed
                            await CompletionCertificate_MT.CopyToAsync(stream);
                        }
                        string uploadPathEntireWOHightingAll_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/WOHAMT");
                        string filePathEntireWOHightingAll_MT = Path.Combine(uploadPathEntireWOHightingAll_MT,
                            fileNameEntireWOHightingAll_MT + extensionEntireWOHightingAll_MT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathEntireWOHightingAll_MT))
                        {
                            // The file is saved in a buffer before being processed
                            await EntireWOHightingAll_MT.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_6s.Add(new A_APA_TE_6
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                ActivityName_1_MT = result.ActivityName_1_MT,
                                ActivityName_2_MT = result.ActivityName_2_MT,
                                ActivityName_3_MT = result.ActivityName_3_MT,
                                ActivityName_4_MT = result.ActivityName_4_MT,
                                ActivityName_5_MT = result.ActivityName_5_MT,
                                ActivityValue_1_MT = result.ActivityValue_1_MT,
                                ActivityValue_2_MT = result.ActivityValue_2_MT,
                                ActivityValue_3_MT = result.ActivityValue_3_MT,
                                ActivityValue_4_MT = result.ActivityValue_4_MT,
                                ActivityValue_5_MT = result.ActivityValue_5_MT,

                                FundofActivity_1_MT = result.FundofActivity_1_MT,
                                FundofActivity_2_MT = result.FundofActivity_2_MT,
                                FundofActivity_3_MT = result.FundofActivity_3_MT,
                                FundofActivity_4_MT = result.FundofActivity_4_MT,
                                FundofActivity_5_MT = result.FundofActivity_5_MT,

                                Activity_1_MT = fileNameActivity_1_MT + extensionActivity_1_MT,
                                Activity_1_MT_Path = filePathActivity_1_MT,
                                Activity_2_MT = fileNameActivity_2_MT + extensionActivity_2_MT,
                                Activity_2_MT_Path = filePathActivity_2_MT,
                                Activity_3_MT = fileNameActivity_3_MT + extensionActivity_3_MT,
                                Activity_3_MT_Path = filePathActivity_3_MT,
                                Activity_4_MT = fileNameActivity_4_MT + extensionActivity_4_MT,
                                Activity_4_MT_Path = filePathActivity_4_MT,
                                Activity_5_MT = fileNameActivity_5_MT + extensionActivity_5_MT,
                                Activity_5_MT_Path = filePathActivity_5_MT,
                                EntireWOHightingAll_MT = fileNameEntireWOHightingAll_MT + extensionEntireWOHightingAll_MT,
                                EntireWOHightingAll_MT_Path = filePathEntireWOHightingAll_MT,
                                CompletionCertificate_MT = fileNameCompletionCertificate_MT + extensionCompletionCertificate_MT,
                                CompletionCertificate_MT_Path = filePathCompletionCertificate_MT,
                                ActiveStatus = 1,
                                User_ID = HttpContext.Session.GetString("UserInfo").ToString(),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_6_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_6", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("GramSavaMeetingResolution & DeclarationofAnnex6", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry sss";
                return RedirectToAction("APA_TE_6", "GPDashboard");
            }
        }
        public IActionResult APA_TE_7()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_7(A_APA_TE_7 result, IFormFile Activity_1_AQT, IFormFile Activity_2_AQT, IFormFile Activity_3_AQT,
    IFormFile Activity_4_AQT, IFormFile Activity_5_AQT, IFormFile CompletionCertificate_AQT, IFormFile EntireWOHightingAll_AQT)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);

            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_7s.Where(q => q.GPCode == gpCode).FirstOrDefault();


            if (data == null && Activity_1_AQT != null && Activity_2_AQT != null && Activity_3_AQT != null && Activity_4_AQT != null &&
                Activity_5_AQT != null && CompletionCertificate_AQT != null && EntireWOHightingAll_AQT != null)
            {
                if (Activity_1_AQT.Length > MaxContentLength && Activity_2_AQT.Length > MaxContentLength &&
                    Activity_3_AQT.Length > MaxContentLength && Activity_4_AQT.Length > MaxContentLength && Activity_5_AQT.Length > MaxContentLength
                    && EntireWOHightingAll_AQT.Length > MaxContentLength && CompletionCertificate_AQT.Length > MaxContentLength)
                {
                    ModelState.AddModelError("Material Testing & Annex, Completion Certificate", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return RedirectToAction("APA_TE_7_Edit", "GPEditDashboard");
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameActivity_1_AQT = gpCode.ToString() + "-" + "aqt_1" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_1_AQT = Path.GetExtension(Activity_1_AQT.FileName);
                    var fileNameActivity_2_AQT = gpCode.ToString() + "-" + "aqt_2" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_2_AQT = Path.GetExtension(Activity_2_AQT.FileName);
                    var fileNameActivity_3_AQT = gpCode.ToString() + "-" + "aqt_3" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_3_AQT = Path.GetExtension(Activity_3_AQT.FileName);
                    var fileNameActivity_4_AQT = gpCode.ToString() + "-" + "aqt_4" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_4_AQT = Path.GetExtension(Activity_4_AQT.FileName);
                    var fileNameActivity_5_AQT = gpCode.ToString() + "-" + "aqt_5" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_5_AQT = Path.GetExtension(Activity_5_AQT.FileName);
                    var fileNameEntireWOHightingAll_AQT = gpCode.ToString() + "-" + "wohallaqt" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionEntireWOHightingAll_AQT = Path.GetExtension(EntireWOHightingAll_AQT.FileName);
                    var fileNameCompletionCertificate_AQT = gpCode.ToString() + "-" + "ccaqt" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionCompletionCertificate_AQT = Path.GetExtension(CompletionCertificate_AQT.FileName);
                    if (extensionActivity_1_AQT == ".pdf" && extensionActivity_2_AQT == ".pdf" && extensionActivity_3_AQT == ".pdf"
                        && extensionActivity_4_AQT == ".pdf" && extensionActivity_5_AQT == ".pdf" && extensionEntireWOHightingAll_AQT == ".xlsx" || extensionEntireWOHightingAll_AQT == ".xls"
                        && extensionCompletionCertificate_AQT == ".pdf")
                    {
                        string uploadPathActivity_1_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/AQT_1");
                        string filePathActivity_1_AQT = Path.Combine(uploadPathActivity_1_AQT,
                            fileNameActivity_1_AQT + extensionActivity_1_AQT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_1_AQT))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_1_AQT.CopyToAsync(stream);
                        }
                        string uploadPathActivity_2_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/AQT_2");
                        string filePathActivity_2_AQT = Path.Combine(uploadPathActivity_2_AQT,
                            fileNameActivity_2_AQT + extensionActivity_2_AQT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_2_AQT))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_2_AQT.CopyToAsync(stream);
                        }
                        string uploadPathActivity_3_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/AQT_3");
                        string filePathActivity_3_AQT = Path.Combine(uploadPathActivity_3_AQT,
                            fileNameActivity_3_AQT + extensionActivity_3_AQT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_3_AQT))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_3_AQT.CopyToAsync(stream);
                        }
                        string uploadPathActivity_4_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/AQT_4");
                        string filePathActivity_4_AQT = Path.Combine(uploadPathActivity_4_AQT,
                            fileNameActivity_4_AQT + extensionActivity_4_AQT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_4_AQT))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_4_AQT.CopyToAsync(stream);
                        }
                        string uploadPathActivity_5_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/AQT_5");
                        string filePathActivity_5_AQT = Path.Combine(uploadPathActivity_5_AQT,
                            fileNameActivity_5_AQT + extensionActivity_5_AQT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_5_AQT))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_5_AQT.CopyToAsync(stream);
                        }
                        string uploadPathCompletionCertificate_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/CCAQT");
                        string filePathCompletionCertificate_AQT = Path.Combine(uploadPathCompletionCertificate_AQT,
                            fileNameCompletionCertificate_AQT + extensionCompletionCertificate_AQT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathCompletionCertificate_AQT))
                        {
                            // The file is saved in a buffer before being processed
                            await CompletionCertificate_AQT.CopyToAsync(stream);
                        }
                        string uploadPathEntireWOHightingAll_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/WOHAAQT");
                        string filePathEntireWOHightingAll_AQT = Path.Combine(uploadPathEntireWOHightingAll_AQT,
                            fileNameEntireWOHightingAll_AQT + extensionEntireWOHightingAll_AQT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathEntireWOHightingAll_AQT))
                        {
                            // The file is saved in a buffer before being processed
                            await EntireWOHightingAll_AQT.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_7s.Add(new A_APA_TE_7
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                ActivityName_1_AQT = result.ActivityName_1_AQT,
                                ActivityName_2_AQT = result.ActivityName_2_AQT,
                                ActivityName_3_AQT = result.ActivityName_3_AQT,
                                ActivityName_4_AQT = result.ActivityName_4_AQT,
                                ActivityName_5_AQT = result.ActivityName_5_AQT,
                                ActivityValue_1_AQT = result.ActivityValue_1_AQT,
                                ActivityValue_2_AQT = result.ActivityValue_2_AQT,
                                ActivityValue_3_AQT = result.ActivityValue_3_AQT,
                                ActivityValue_4_AQT = result.ActivityValue_4_AQT,
                                ActivityValue_5_AQT = result.ActivityValue_5_AQT,

                                FundofActivity_1_AQT = result.FundofActivity_1_AQT,
                                FundofActivity_2_AQT = result.FundofActivity_2_AQT,
                                FundofActivity_3_AQT = result.FundofActivity_3_AQT,
                                FundofActivity_4_AQT = result.FundofActivity_4_AQT,
                                FundofActivity_5_AQT = result.FundofActivity_5_AQT,

                                Activity_1_AQT = fileNameActivity_1_AQT + extensionActivity_1_AQT,
                                Activity_1_AQT_Path = filePathActivity_1_AQT,
                                Activity_2_AQT = fileNameActivity_2_AQT + extensionActivity_2_AQT,
                                Activity_2_AQT_Path = filePathActivity_2_AQT,
                                Activity_3_AQT = fileNameActivity_3_AQT + extensionActivity_3_AQT,
                                Activity_3_AQT_Path = filePathActivity_3_AQT,
                                Activity_4_AQT = fileNameActivity_4_AQT + extensionActivity_4_AQT,
                                Activity_4_AQT_Path = filePathActivity_4_AQT,
                                Activity_5_AQT = fileNameActivity_5_AQT + extensionActivity_5_AQT,
                                Activity_5_AQT_Path = filePathActivity_5_AQT,
                                EntireWOHightingAll_AQT = fileNameEntireWOHightingAll_AQT + extensionEntireWOHightingAll_AQT,
                                EntireWOHightingAll_AQT_Path = filePathEntireWOHightingAll_AQT,
                                CompletionCertificate_AQT = fileNameCompletionCertificate_AQT + extensionCompletionCertificate_AQT,
                                CompletionCertificate_AQT_Path = filePathCompletionCertificate_AQT,
                                ActiveStatus = 1,
                                User_ID = HttpContext.Session.GetString("UserInfo").ToString(),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_7_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_7", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("GramSavaMeetingResolution & DeclarationofAnnex6", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry sss";
                return RedirectToAction("APA_TE_7", "GPDashboard");
            }
        }
        public IActionResult APA_TE_8()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_8(A_APA_TE_8 result, IFormFile Activity_1_WT)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_8s.Where(q => q.GPCode == gpCode).FirstOrDefault();

            if (data == null && Activity_1_WT != null)
            {
                if (Activity_1_WT.Length > MaxContentLength)
                {
                    ModelState.AddModelError("Activity_1_WT", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameActivity_1_WT = gpCode.ToString() + "-" + "wt" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    var extensionActivity_1_WT = Path.GetExtension(Activity_1_WT.FileName);
                    if (extensionActivity_1_WT == ".pdf")
                    {
                        string uploadPathActivity_1_WT = Path.Combine(_environment.WebRootPath, "storages/apa_te_8/WT");
                        string filePathActivity_1_WT = Path.Combine(uploadPathActivity_1_WT,
                            fileNameActivity_1_WT + extensionActivity_1_WT);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_1_WT))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_1_WT.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_8s.Add(new A_APA_TE_8()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                TotalNoofWTDone = result.TotalNoofWTDone,
                                Activity_1_WT = fileNameActivity_1_WT + extensionActivity_1_WT,
                                Activity_1_WT_Path = filePathActivity_1_WT,
                                ActiveStatus = 1,
                                User_ID = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_8_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_8", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("Activity_1_WT", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_8", "GPDashboard");
            }
        }
        public IActionResult APA_TE_9()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_9(A_APA_TE_9 result, IFormFile WOActivityActivitieshavingvaluesmorethanorequalstwolakh, IFormFile InspectionReportActivitiesEstimate)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_9s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && WOActivityActivitieshavingvaluesmorethanorequalstwolakh != null && InspectionReportActivitiesEstimate != null)
            {
                if (WOActivityActivitieshavingvaluesmorethanorequalstwolakh.Length > MaxContentLength && InspectionReportActivitiesEstimate.Length > MaxContentLength)
                {
                    ModelState.AddModelError("WOActivityActivitieshavingvaluesmorethanorequalstwolakh && InspectionReportActivitiesEstimate", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameWOActivityActivitieshavingvaluesmorethanorequalstwolakh = gpCode.ToString() + "-" + "woahv" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionWOActivityActivitieshavingvaluesmorethanorequalstwolakh = Path.GetExtension(WOActivityActivitieshavingvaluesmorethanorequalstwolakh.FileName);
                    var fileNameInspectionReportActivitiesEstimate = gpCode.ToString() + "-" + "irae" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionInspectionReportActivitiesEstimate = Path.GetExtension(InspectionReportActivitiesEstimate.FileName);
                    if (extensionWOActivityActivitieshavingvaluesmorethanorequalstwolakh == ".xlsx" || extensionWOActivityActivitieshavingvaluesmorethanorequalstwolakh == ".xls" && extensionInspectionReportActivitiesEstimate == ".pdf")
                    {
                        string uploadPathWOActivityActivitieshavingvaluesmorethanorequalstwolakh = Path.Combine(_environment.WebRootPath, "storages/apa_te_9/WOAHV");
                        string filePathWOActivityActivitieshavingvaluesmorethanorequalstwolakh = Path.Combine(uploadPathWOActivityActivitieshavingvaluesmorethanorequalstwolakh,
                            fileNameWOActivityActivitieshavingvaluesmorethanorequalstwolakh + extensionWOActivityActivitieshavingvaluesmorethanorequalstwolakh);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathWOActivityActivitieshavingvaluesmorethanorequalstwolakh))
                        {
                            // The file is saved in a buffer before being processed
                            await WOActivityActivitieshavingvaluesmorethanorequalstwolakh.CopyToAsync(stream);
                        }
                        string uploadPathInspectionReportActivitiesEstimate = Path.Combine(_environment.WebRootPath, "storages/apa_te_9/IRAE");
                        string filePathInspectionReportActivitiesEstimate = Path.Combine(uploadPathInspectionReportActivitiesEstimate,
                            fileNameInspectionReportActivitiesEstimate + extensionInspectionReportActivitiesEstimate);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathInspectionReportActivitiesEstimate))
                        {
                            // The file is saved in a buffer before being processed
                            await InspectionReportActivitiesEstimate.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_9s.Add(new A_APA_TE_9()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                TotalNoofInspectedActivitiesmorethanorequalstwolakh = result.TotalNoofInspectedActivitiesmorethanorequalstwolakh,
                                TotalNoofActivitieshavingvaluesmorethanorequalstwolakh = result.TotalNoofActivitieshavingvaluesmorethanorequalstwolakh,
                                WOActivityActivitieshavingvaluesmorethanorequalstwolakh = fileNameWOActivityActivitieshavingvaluesmorethanorequalstwolakh + extensionWOActivityActivitieshavingvaluesmorethanorequalstwolakh,
                                WOActivityActivitieshavingvaluesmorethanorequalstwolakh_path = filePathWOActivityActivitieshavingvaluesmorethanorequalstwolakh,
                                InspectionReportActivitiesEstimate = fileNameInspectionReportActivitiesEstimate + extensionInspectionReportActivitiesEstimate,
                                InspectionReportActivitiesEstimate_path = filePathInspectionReportActivitiesEstimate,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_9_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_9", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("WOActivityActivitieshavingvaluesmorethanorequalstwolakh && InspectionReportActivitiesEstimate", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_8", "GPDashboard");
            }
        }
        public IActionResult APA_TE_10()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_10(A_APA_TE_10 result, IFormFile ResolutionDoconForm27)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_10s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && ResolutionDoconForm27 != null)
            {
                if (ResolutionDoconForm27.Length > MaxContentLength)
                {
                    ModelState.AddModelError("ResolutionDoconForm27", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameResolutionDoconForm27 = gpCode.ToString() + "-" + "form27" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionResolutionDoconForm27 = Path.GetExtension(ResolutionDoconForm27.FileName);
                    if (extensionResolutionDoconForm27 == ".pdf")
                    {
                        string uploadPathResolutionDoconForm27 = Path.Combine(_environment.WebRootPath, "storages/apa_te_10/FORM27");
                        string filePathResolutionDoconForm27 = Path.Combine(uploadPathResolutionDoconForm27,
                            fileNameResolutionDoconForm27 + extensionResolutionDoconForm27);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathResolutionDoconForm27))
                        {
                            // The file is saved in a buffer before being processed
                            await ResolutionDoconForm27.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_10s.Add(new A_APA_TE_10()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                GBMeetingDate = result.GBMeetingDate,
                                Form27Approved = result.Form27Approved,
                                ResolutionDoconForm27 = fileNameResolutionDoconForm27 + extensionResolutionDoconForm27,
                                ResolutionDoconForm27_path = filePathResolutionDoconForm27,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_10_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_10", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("ResolutionDoconForm27", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_10", "GPDashboard");
            }
        }
        public IActionResult APA_TE_13()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_13(A_APA_TE_13 result, IFormFile Activity_1_CFCSFC, IFormFile Activity_2_CFCSFC, IFormFile Activity_3_CFCSFC,
    IFormFile Activity_4_CFCSFC, IFormFile Activity_5_CFCSFC, IFormFile EntireWOHightingAll_CFCSFC)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_13s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && Activity_1_CFCSFC != null && Activity_2_CFCSFC != null && Activity_3_CFCSFC != null && Activity_4_CFCSFC != null &&
                Activity_5_CFCSFC != null && EntireWOHightingAll_CFCSFC != null)
            {
                if (Activity_1_CFCSFC.Length > MaxContentLength && Activity_2_CFCSFC.Length > MaxContentLength &&
                    Activity_3_CFCSFC.Length > MaxContentLength && Activity_4_CFCSFC.Length > MaxContentLength && Activity_5_CFCSFC.Length > MaxContentLength
                    && EntireWOHightingAll_CFCSFC.Length > MaxContentLength)
                {
                    ModelState.AddModelError("SFC CFC Activities & Annex-II, Completion Certificate", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameActivity_1_CFCSFC = gpCode.ToString() + "-" + "cfcsfc_1" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_1_CFCSFC = Path.GetExtension(Activity_1_CFCSFC.FileName);
                    var fileNameActivity_2_CFCSFC = gpCode.ToString() + "-" + "cfcsfc_2" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_2_CFCSFC = Path.GetExtension(Activity_2_CFCSFC.FileName);
                    var fileNameActivity_3_CFCSFC = gpCode.ToString() + "-" + "cfcsfc_3" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_3_CFCSFC = Path.GetExtension(Activity_3_CFCSFC.FileName);
                    var fileNameActivity_4_CFCSFC = gpCode.ToString() + "-" + "cfcsfc_4" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_4_CFCSFC = Path.GetExtension(Activity_4_CFCSFC.FileName);
                    var fileNameActivity_5_CFCSFC = gpCode.ToString() + "-" + "cfcsfc_5" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionActivity_5_CFCSFC = Path.GetExtension(Activity_5_CFCSFC.FileName);
                    var fileNameEntireWOHightingAll_CFCSFC = gpCode.ToString() + "-" + "wohall" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionEntireWOHightingAll_CFCSFC = Path.GetExtension(EntireWOHightingAll_CFCSFC.FileName);
                    if (extensionActivity_1_CFCSFC == ".pdf" && extensionActivity_2_CFCSFC == ".pdf" && extensionActivity_3_CFCSFC == ".pdf"
                        && extensionActivity_4_CFCSFC == ".pdf" && extensionActivity_5_CFCSFC == ".pdf" && extensionEntireWOHightingAll_CFCSFC == ".xlsx" || extensionEntireWOHightingAll_CFCSFC == ".xls"
                        )
                    {
                        string uploadPathActivity_1_CFCSFC = Path.Combine(_environment.WebRootPath, "storages/apa_te_13/CFCSFC_1");
                        string filePathActivity_1_CFCSFC = Path.Combine(uploadPathActivity_1_CFCSFC,
                            fileNameActivity_1_CFCSFC + extensionActivity_1_CFCSFC);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_1_CFCSFC))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_1_CFCSFC.CopyToAsync(stream);
                        }
                        string uploadPathActivity_2_CFCSFC = Path.Combine(_environment.WebRootPath, "storages/apa_te_13/CFCSFC_2");
                        string filePathActivity_2_CFCSFC = Path.Combine(uploadPathActivity_2_CFCSFC,
                            fileNameActivity_2_CFCSFC + extensionActivity_2_CFCSFC);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_2_CFCSFC))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_2_CFCSFC.CopyToAsync(stream);
                        }
                        string uploadPathActivity_3_CFCSFC = Path.Combine(_environment.WebRootPath, "storages/apa_te_13/CFCSFC_3");
                        string filePathActivity_3_CFCSFC = Path.Combine(uploadPathActivity_3_CFCSFC,
                            fileNameActivity_3_CFCSFC + extensionActivity_3_CFCSFC);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_3_CFCSFC))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_3_CFCSFC.CopyToAsync(stream);
                        }
                        string uploadPathActivity_4_CFCSFC = Path.Combine(_environment.WebRootPath, "storages/apa_te_13/CFCSFC_4");
                        string filePathActivity_4_CFCSFC = Path.Combine(uploadPathActivity_4_CFCSFC,
                            fileNameActivity_4_CFCSFC + extensionActivity_4_CFCSFC);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_4_CFCSFC))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_4_CFCSFC.CopyToAsync(stream);
                        }
                        string uploadPathActivity_5_CFCSFC = Path.Combine(_environment.WebRootPath, "storages/apa_te_13/CFCSFC_5");
                        string filePathActivity_5_CFCSFC = Path.Combine(uploadPathActivity_5_CFCSFC,
                            fileNameActivity_5_CFCSFC + extensionActivity_5_CFCSFC);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathActivity_5_CFCSFC))
                        {
                            // The file is saved in a buffer before being processed
                            await Activity_5_CFCSFC.CopyToAsync(stream);
                        }
                        string uploadPathEntireWOHightingAll_CFCSFC = Path.Combine(_environment.WebRootPath, "storages/apa_te_13/WOHA");
                        string filePathEntireWOHightingAll_CFCSFC = Path.Combine(uploadPathEntireWOHightingAll_CFCSFC,
                            fileNameEntireWOHightingAll_CFCSFC + extensionEntireWOHightingAll_CFCSFC);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathEntireWOHightingAll_CFCSFC))
                        {
                            // The file is saved in a buffer before being processed
                            await EntireWOHightingAll_CFCSFC.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_13s.Add(new A_APA_TE_13
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                ActivityName_1_CFCSFC = result.ActivityName_1_CFCSFC,
                                ActivityName_2_CFCSFC = result.ActivityName_2_CFCSFC,
                                ActivityName_3_CFCSFC = result.ActivityName_3_CFCSFC,
                                ActivityName_4_CFCSFC = result.ActivityName_4_CFCSFC,
                                ActivityName_5_CFCSFC = result.ActivityName_5_CFCSFC,
                                ActivityValue_1_CFCSFC = result.ActivityValue_1_CFCSFC,
                                ActivityValue_2_CFCSFC = result.ActivityValue_2_CFCSFC,
                                ActivityValue_3_CFCSFC = result.ActivityValue_3_CFCSFC,
                                ActivityValue_4_CFCSFC = result.ActivityValue_4_CFCSFC,
                                ActivityValue_5_CFCSFC = result.ActivityValue_5_CFCSFC,

                                FundofActivity_1_CFCSFC = result.FundofActivity_1_CFCSFC,
                                FundofActivity_2_CFCSFC = result.FundofActivity_2_CFCSFC,
                                FundofActivity_3_CFCSFC = result.FundofActivity_3_CFCSFC,
                                FundofActivity_4_CFCSFC = result.FundofActivity_4_CFCSFC,
                                FundofActivity_5_CFCSFC = result.FundofActivity_5_CFCSFC,

                                Activity_1_CFCSFC = fileNameActivity_1_CFCSFC + extensionActivity_1_CFCSFC,
                                Activity_1_CFCSFC_Path = filePathActivity_1_CFCSFC,
                                Activity_2_CFCSFC = fileNameActivity_2_CFCSFC + extensionActivity_2_CFCSFC,
                                Activity_2_CFCSFC_Path = filePathActivity_2_CFCSFC,
                                Activity_3_CFCSFC = fileNameActivity_3_CFCSFC + extensionActivity_3_CFCSFC,
                                Activity_3_CFCSFC_Path = filePathActivity_3_CFCSFC,
                                Activity_4_CFCSFC = fileNameActivity_4_CFCSFC + extensionActivity_4_CFCSFC,
                                Activity_4_CFCSFC_Path = filePathActivity_4_CFCSFC,
                                Activity_5_CFCSFC = fileNameActivity_5_CFCSFC + extensionActivity_5_CFCSFC,
                                Activity_5_CFCSFC_Path = filePathActivity_5_CFCSFC,
                                EntireWOHightingAll_CFCSFC = fileNameEntireWOHightingAll_CFCSFC + extensionEntireWOHightingAll_CFCSFC,
                                EntireWOHightingAll_CFCSFC_Path = filePathEntireWOHightingAll_CFCSFC,
                                ActiveStatus = 1,
                                User_ID = HttpContext.Session.GetString("UserInfo").ToString(),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_13_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_13", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("GramSavaMeetingResolution & DeclarationofAnnex6", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_13", "GPDashboard");
            }
        }
        public IActionResult APA_TE_14()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                var info = _context.A_APA_MC_6s.Where(q => q.GPCode == data).FirstOrDefault();
                if(info != null)
                {
                    ViewBag.Info = info.TotalTaxinINR_2425;
                }
                else
                {
                    ViewBag.Info = 0;
                }
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_14(A_APA_TE_14 result, IFormFile Form7Upload)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_14s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && Form7Upload != null)
            {
                if (Form7Upload.Length > MaxContentLength)
                {
                    ModelState.AddModelError("Form7Upload", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameForm7Upload = gpCode.ToString() + "-" + "form7" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionForm7Upload = Path.GetExtension(Form7Upload.FileName);
                    if (extensionForm7Upload == ".pdf")
                    {
                        string uploadPathForm7Upload = Path.Combine(_environment.WebRootPath, "storages/apa_te_14/FORM7");
                        string filePathForm7Upload = Path.Combine(uploadPathForm7Upload,
                            fileNameForm7Upload + extensionForm7Upload);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathForm7Upload))
                        {
                            // The file is saved in a buffer before being processed
                            await Form7Upload.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_14s.Add(new A_APA_TE_14()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                Tax_non_tax_collected_ownsource_revenue = result.Tax_non_tax_collected_ownsource_revenue,
                                TotalOSRDemandCount = result.TotalOSRDemandCount,
                                Form7Upload = fileNameForm7Upload + extensionForm7Upload,
                                Form7Upload_path = filePathForm7Upload,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_14_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_14", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("Form7Upload", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_14", "GPDashboard");
            }
        }
        public IActionResult APA_TE_16()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                var apa6Data = _context.A_APA_MC_6s.Where(q => q.GPCode == data).FirstOrDefault();
                if(apa6Data != null)
                {
                    ViewBag.Info = apa6Data.TotalOSRinINR_2425;
                }
                else
                {
                    ViewBag.Info = 0;
                }
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_16(A_APA_TE_16 result, IFormFile Upload_Annex_7)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_16s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && Upload_Annex_7 != null)
            {
                if (Upload_Annex_7.Length > MaxContentLength)
                {
                    ModelState.AddModelError("Form7Upload", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameUpload_Annex_7 = gpCode.ToString() + "-" + "annex7" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionUpload_Annex_7 = Path.GetExtension(Upload_Annex_7.FileName);
                    if (extensionUpload_Annex_7 == ".xlsx" || extensionUpload_Annex_7 == ".xls")
                    {
                        string uploadPathUpload_Annex_7 = Path.Combine(_environment.WebRootPath, "storages/apa_te_16/ANNEX7");
                        string filePathForm7Upload = Path.Combine(uploadPathUpload_Annex_7,
                            fileNameUpload_Annex_7 + extensionUpload_Annex_7);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathForm7Upload))
                        {
                            // The file is saved in a buffer before being processed
                            await Upload_Annex_7.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_16s.Add(new A_APA_TE_16()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                OSR_Collected_202425 = result.OSR_Collected_202425,
                                OSR_Utilized_Development = result.OSR_Utilized_Development,
                                Upload_Annex_7 = fileNameUpload_Annex_7 + extensionUpload_Annex_7,
                                Upload_Annex_7_Path = filePathForm7Upload,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_16_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_16", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("Annecure 7", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_16", "GPDashboard");
            }
        }
        public IActionResult APA_TE_17()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_17(A_APA_TE_17 result, IFormFile UploadMeetingResolutionConducted202425)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_17s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && UploadMeetingResolutionConducted202425 != null)
            {
                if (UploadMeetingResolutionConducted202425.Length > MaxContentLength)
                {
                    ModelState.AddModelError("UploadMeetingResolutionConducted202425", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameUploadMeetingResolutionConducted202425 = gpCode.ToString() + "-" + "mrc" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionUploadMeetingResolutionConducted202425 = Path.GetExtension(UploadMeetingResolutionConducted202425.FileName);
                    if (extensionUploadMeetingResolutionConducted202425 == ".pdf")
                    {
                        string uploadPathUploadMeetingResolutionConducted202425 = Path.Combine(_environment.WebRootPath, "storages/apa_te_17/MRC");
                        string filePathUploadMeetingResolutionConducted202425 = Path.Combine(uploadPathUploadMeetingResolutionConducted202425,
                            fileNameUploadMeetingResolutionConducted202425 + extensionUploadMeetingResolutionConducted202425);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathUploadMeetingResolutionConducted202425))
                        {
                            // The file is saved in a buffer before being processed
                            await UploadMeetingResolutionConducted202425.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_17s.Add(new A_APA_TE_17()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                TotalMeetingConducted202425 = result.TotalMeetingConducted202425,
                                UploadMeetingResolutionConducted202425 = fileNameUploadMeetingResolutionConducted202425 + extensionUploadMeetingResolutionConducted202425,
                                UploadMeetingResolutionConducted202425_Path = filePathUploadMeetingResolutionConducted202425,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_17_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_17", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("UploadMeetingResolutionConducted202425", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_17", "GPDashboard");
            }
        }
        public IActionResult APA_TE_19()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                var mc_3 = _context.A_APA_MC_3s.Where(q => q.GPCode == accessID.GPCode).FirstOrDefault();
                if (accessID != null)
                {
                    if(mc_3 != null)
                    {
                        ViewBag.SFC_Exp = mc_3.EXP_SFC_UPTO_MAR_2025;
                        ViewBag.OSR_Exp = mc_3.EXP_OSR_UPTO_MAR_2025;
                        ViewBag.GPName = accessID.GPName;
                        ViewBag.GPCode = accessID.GPCode;
                        ViewBag.FYCode = fin.FYCode;
                        ViewBag.FYName = fin.FYName;
                    }
                    else
                    {
                        TempData["Failed"] = "No data found";
                    }                        
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_19(A_APA_TE_19 result, IFormFile UploadAnnex8_PDF, IFormFile UploadAnnex8_Excel)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_19s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && UploadAnnex8_PDF != null && UploadAnnex8_Excel != null)
            {
                if (UploadAnnex8_Excel.Length > MaxContentLength && UploadAnnex8_PDF.Length > MaxContentLength)
                {
                    ModelState.AddModelError("UploadAnnex8_Excel && UploadAnnex8_PDF", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameUploadAnnex8_Excel = gpCode.ToString() + "-" + "annexexcel" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionUploadAnnex8_Excel = Path.GetExtension(UploadAnnex8_Excel.FileName);
                    var fileNameUploadAnnex8_PDF = gpCode.ToString() + "-" + "annexpdf" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionUploadAnnex8_PDF = Path.GetExtension(UploadAnnex8_PDF.FileName);
                    if (extensionUploadAnnex8_Excel == ".xlsx" || extensionUploadAnnex8_Excel == ".xls" && extensionUploadAnnex8_PDF == ".pdf")
                    {
                        string uploadPathUploadAnnex8_Excel = Path.Combine(_environment.WebRootPath, "storages/apa_te_19/ANNEXEXCEL");
                        string filePathUploadAnnex8_Excel = Path.Combine(uploadPathUploadAnnex8_Excel,
                            fileNameUploadAnnex8_Excel + extensionUploadAnnex8_Excel);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathUploadAnnex8_Excel))
                        {
                            // The file is saved in a buffer before being processed
                            await UploadAnnex8_Excel.CopyToAsync(stream);
                        }
                        string uploadPathUploadAnnex8_PDF = Path.Combine(_environment.WebRootPath, "storages/apa_te_19/ANNEXPDF");
                        string filePathUploadAnnex8_PDF = Path.Combine(uploadPathUploadAnnex8_PDF,
                            fileNameUploadAnnex8_PDF + extensionUploadAnnex8_PDF);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathUploadAnnex8_PDF))
                        {
                            // The file is saved in a buffer before being processed
                            await UploadAnnex8_PDF.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_19s.Add(new A_APA_TE_19()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                ExpenditureforSFC = result.ExpenditureforSFC,
                                ExpenditureforCFCUntied = result.ExpenditureforCFCUntied,
                                ExpenditureforOSR = result.ExpenditureforOSR,
                                UntiedAmountSpentfromCFCSFCOSR202425 = result.UntiedAmountSpentfromCFCSFCOSR202425,
                                UploadAnnex8_PDF = fileNameUploadAnnex8_PDF + extensionUploadAnnex8_PDF,
                                UploadAnnex8_PDF_path = filePathUploadAnnex8_PDF,
                                UploadAnnex8_Excel = fileNameUploadAnnex8_Excel + extensionUploadAnnex8_Excel,
                                UploadAnnex8_Excel_path = filePathUploadAnnex8_Excel,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_19_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_19", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("UploadAnnex8_Excel & UploadAnnex8_PDF", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_19", "GPDashboard");
            }
        }
        
        public IActionResult APA_TE_11()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_11(A_APA_TE_11 result, IFormFile ATRResolutionSubmissionDoc)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_11s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && ATRResolutionSubmissionDoc != null)
            {
                if (ATRResolutionSubmissionDoc.Length > MaxContentLength)
                {
                    ModelState.AddModelError("UploadATR", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions_pdf = new[] { ".pdf" };
                    var fileNameATRResolutionSubmissionDoc = gpCode.ToString() + "-" + "atr" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionATRResolutionSubmissionDoc = Path.GetExtension(ATRResolutionSubmissionDoc.FileName);
                    if (extensionATRResolutionSubmissionDoc == ".pdf")
                    {
                        string uploadPathATRResolutionSubmissionDoc = Path.Combine(_environment.WebRootPath, "storages/apa_te_11/ATR");
                        string filePathATRResolutionSubmissionDoc = Path.Combine(uploadPathATRResolutionSubmissionDoc,
                            fileNameATRResolutionSubmissionDoc + extensionATRResolutionSubmissionDoc);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathATRResolutionSubmissionDoc))
                        {
                            // The file is saved in a buffer before being processed
                            await ATRResolutionSubmissionDoc.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_11s.Add(new A_APA_TE_11()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                ATRResolutionSubmissionDoc = fileNameATRResolutionSubmissionDoc + extensionATRResolutionSubmissionDoc,
                                ATRResolutionSubmissionDoc_path = filePathATRResolutionSubmissionDoc,
                                ATRSubmissionDate = result.ATRSubmissionDate,
                                LastAuditReportReceivedDate = result.LastAuditReportReceivedDate,
                                MeetingDateonATR = result.MeetingDateonATR,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_11_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_11", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("UploadATR", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_11", "GPDashboard");
            }
        }
        /*public IActionResult APA_TE_18()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_18(A_APA_TE_18 result)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            var data = _context.A_APA_TE_18s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null)
            {
                var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                try
                {
                    _context.A_APA_TE_18s.Add(new A_APA_TE_18()
                    {
                        GPCode = result.GPCode,
                        FYCode = result.FYCode,
                        ReportedExpenditureupto20242025 = result.ReportedExpenditureupto20242025,
                        Expenditureupto20242025takenfromMISTools = result.Expenditureupto20242025takenfromMISTools,
                        ActiveStatus = 1,
                        User_Id = HttpContext.Session.GetString("UserInfo"),
                        Entry_Time = DateTime.Now
                    });
                    _context.SaveChanges();
                    TempData["Success"] = "Your Date is updated successfully!";
                    return RedirectToAction("APA_TE_18_Report", "GPReport");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return RedirectToAction("APA_TE_18", "GPDashboard");
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_18", "GPDashboard");
            }
        }*/
        public IActionResult APA_TE_20()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                //var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_20(A_APA_TE_20 result, IFormFile GrievenceImage)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 6;
            var data = _context.A_APA_TE_20s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && GrievenceImage != null)
            {
                if (GrievenceImage.Length > MaxContentLength)
                {
                    ModelState.AddModelError("GrievenceImage", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var fileNameGrievenceImage = gpCode.ToString() + "-" + "griev" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionGrievenceImage = Path.GetExtension(GrievenceImage.FileName);
                    if (extensionGrievenceImage == ".jpg" || extensionGrievenceImage == ".jpeg")
                    {
                        string uploadPathGrievenceImage = Path.Combine(_environment.WebRootPath, "storages/apa_te_20/Image");
                        string filePathGrievenceImage = Path.Combine(uploadPathGrievenceImage,
                            fileNameGrievenceImage + extensionGrievenceImage);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathGrievenceImage))
                        {
                            // The file is saved in a buffer before being processed
                            await GrievenceImage.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_TE_20s.Add(new A_APA_TE_20()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                GrievenceLogged = result.GrievenceLogged,
                                GrievenceResolved = result.GrievenceResolved,
                                GrievenceImage = fileNameGrievenceImage + extensionGrievenceImage,
                                GrievenceImage_Path = filePathGrievenceImage,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your File is uploaded successfully!";
                            return RedirectToAction("APA_TE_20_Report", "GPReport");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_TE_20", "GPDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("GrievenceImage", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_TE_20", "GPDashboard");
            }
        }

        /// New Update
        /// 
        public IActionResult APA_MC_5()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "GPAdmin")
            {
                var data = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var accessID = _context.view_alllocations.Where(q => q.GPCode == data).FirstOrDefault();
                var fin = _context.mst_FinancialYears.OrderByDescending(q => q.FYCode).Skip(1).Take(1).FirstOrDefault();
                if (accessID != null)
                {
                    ViewBag.GPName = accessID.GPName;
                    ViewBag.GPCode = accessID.GPCode;
                    ViewBag.FYCode = fin.FYCode;
                    ViewBag.FYName = fin.FYName;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_MC_5(A_APA_MC_5 result, IFormFile AuditCertificate)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_MC_5s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if(data == null && AuditCertificate != null)
            {
                if (AuditCertificate.Length > MaxContentLength)
                {
                    ModelState.AddModelError("AuditCertificate & ATRResolutionSubmissionDoc", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return RedirectToAction("APA_MC_5", "GPDashboard");
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var fileNameAuditCertificate = gpCode.ToString() + "-" + "audit" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionAuditCertificate = Path.GetExtension(AuditCertificate.FileName);
                    if (extensionAuditCertificate == ".pdf")
                    {
                        string uploadPathAuditCertificate = Path.Combine(_environment.WebRootPath, "storages/apa_mc_5/AUDIT");
                        string filePathAuditCertificate = Path.Combine(uploadPathAuditCertificate,
                            fileNameAuditCertificate + extensionAuditCertificate);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathAuditCertificate))
                        {
                            // The file is saved in a buffer before being processed
                            await AuditCertificate.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_MC_5s.Add(new A_APA_MC_5()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                AuditOpinion = result.AuditOpinion,
                                AuditCertificate= fileNameAuditCertificate + extensionAuditCertificate,
                                AuditCertificate_Path = filePathAuditCertificate,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your Report Uploaded Successfully!";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_MC_5_Report", "GPReport");
                    }
                    else
                    {
                        ModelState.AddModelError("AuditCertificate", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_MC_5", "GPDashboard");
            }
        }
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_MC_5_TE_11(A_APA_MC_5_TE_11 result, IFormFile EvidenceofAuditReportReceived, IFormFile ATRResolutionSubmissionDoc)
        {
            var fycode = Convert.ToInt32(result.FYCode);
            var gpCode = Convert.ToInt32(result.GPCode);
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_MC_5_TE_11s.Where(q => q.GPCode == gpCode).FirstOrDefault();
            if (data == null && EvidenceofAuditReportReceived != null && ATRResolutionSubmissionDoc != null)
            {
                if (EvidenceofAuditReportReceived.Length > MaxContentLength && ATRResolutionSubmissionDoc.Length > MaxContentLength)
                {
                    ModelState.AddModelError("EvidenceofAuditReportReceived & ATRResolutionSubmissionDoc", "File size is big.");
                    TempData["Failed"] = "Your Report could not uploaded! File size is larger than given side.";
                    return View(result);
                }
                else
                {
                    var finData = _context.mst_FinancialYears.Where(q => q.FYCode.Equals(fycode)).FirstOrDefault();
                    var loc = _context.view_alllocations.Where(q => q.GPCode.Equals(gpCode)).FirstOrDefault();
                    var allowedExtensions = new[] { ".pdf" };

                    var fileNameEvidenceofAuditReportReceived = gpCode.ToString() + "-" + "eoarr" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var fileNameATRResolutionSubmissionDoc = gpCode.ToString() + "-" + "arsd" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var extensionEvidenceofAuditReportReceived = Path.GetExtension(EvidenceofAuditReportReceived.FileName);
                    var extensionATRResolutionSubmissionDoc = Path.GetExtension(ATRResolutionSubmissionDoc.FileName);

                    if (extensionEvidenceofAuditReportReceived == ".pdf" && extensionATRResolutionSubmissionDoc == ".pdf")
                    {
                        string uploadPathEvidenceofAuditReportReceived = Path.Combine(_environment.WebRootPath, "storages/apa_mc_5_te_11/EOARR");
                        string filePathEvidenceofAuditReportReceived = Path.Combine(uploadPathEvidenceofAuditReportReceived,
                            fileNameEvidenceofAuditReportReceived + extensionEvidenceofAuditReportReceived);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathEvidenceofAuditReportReceived))
                        {
                            // The file is saved in a buffer before being processed
                            await EvidenceofAuditReportReceived.CopyToAsync(stream);
                        }
                        string uploadPathATRResolutionSubmissionDoc = Path.Combine(_environment.WebRootPath, "storages/apa_mc_5_te_11/ARSD");
                        string filePathATRResolutionSubmissionDoc = Path.Combine(uploadPathATRResolutionSubmissionDoc,
                            fileNameATRResolutionSubmissionDoc + extensionATRResolutionSubmissionDoc);
                        //Using Buffering
                        using (var stream = System.IO.File.Create(filePathATRResolutionSubmissionDoc))
                        {
                            // The file is saved in a buffer before being processed
                            await ATRResolutionSubmissionDoc.CopyToAsync(stream);
                        }
                        try
                        {
                            _context.A_APA_MC_5_TE_11s.Add(new A_APA_MC_5_TE_11()
                            {
                                GPCode = result.GPCode,
                                FYCode = result.FYCode,
                                AuditOpinion = result.AuditOpinion,
                                LastAuditReportReceivedDate = result.LastAuditReportReceivedDate,
                                EvidenceofAuditReportReceived = fileNameEvidenceofAuditReportReceived + extensionEvidenceofAuditReportReceived,
                                EvidenceofAuditReportReceived_path = filePathEvidenceofAuditReportReceived,
                                MeetingDateonATR = result.MeetingDateonATR,
                                ATRResolutionSubmissionDoc = fileNameATRResolutionSubmissionDoc + extensionATRResolutionSubmissionDoc,
                                ATRResolutionSubmissionDoc_path = filePathATRResolutionSubmissionDoc,
                                ATRSubmissionDate = result.ATRSubmissionDate,
                                ActiveStatus = 1,
                                User_Id = HttpContext.Session.GetString("UserInfo"),
                                Entry_Time = DateTime.Now
                            });
                            _context.SaveChanges();
                            TempData["Success"] = "Your Report Uploaded Successfully!";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        return RedirectToAction("APA_MC_5_TE_11_Report", "GPReport");
                    }
                    else
                    {
                        ModelState.AddModelError("EvidenceofPlanApprovalResolution & TotalMembersDeclaration", "Unsupported file type.");
                        TempData["Failed"] = "Your Report could not uploaded! Unsupported file type.";
                        return View(result);
                    }
                }
            }
            else
            {
                TempData["Failed"] = "Your Reports could not uploaded! You already did your entry";
                return RedirectToAction("APA_MC_5_TE_11", "GPDashboard");
            }
        }*/

    }
}
