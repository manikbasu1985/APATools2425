using APATools.Context;
using APATools.Controllers.GP;
using APATools.Models;
using APATools.OldContext;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APATools.Controllers.State
{
    public class StateEditDashboardController : Controller
    {
        private readonly ILogger<StateEditDashboardController> _logger;
        private readonly APAToolsContext _context;
        private readonly GPIMSContext _gpimsContext;
        private readonly IWebHostEnvironment _environment;
        public StateEditDashboardController(ILogger<StateEditDashboardController> logger, APAToolsContext context, GPIMSContext gPIMSContext, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
            _gpimsContext = gPIMSContext;
        }
        private bool RemoveFileFromServer(string path)
        {
            string fullPath = Path.Combine(_environment.WebRootPath, "storages", path);
            if (!System.IO.File.Exists(fullPath)) return false;

            try //Maybe error could happen like Access denied or Presses Already User used
            {
                System.IO.File.Delete(fullPath);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
        public async Task<IActionResult> APA_MC_1_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_MC_1s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_MC_1_State_Edit", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        /**
         APA_MC_1_State_Edit
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_MC_1_State_Edit(long ID, A_APA_MC_1 result, 
            IFormFile EvidenceofPlanApprovalResolution, IFormFile TotalMembersDeclaration_pdf, IFormFile TotalMembersDeclaration_excel)
        {
            int MaxContentLength = 1024 * 1024 * 6;
            var data = _context.A_APA_MC_1s.Where(q=> q.ID ==  ID).FirstOrDefault();
            if (data == null) return NotFound();
            //Data Upload in Database
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.TotalNoofMember = result.TotalNoofMember;
            data.NoofMemberAttended = result.NoofMemberAttended;
            data.PlanApprovalDate = result.PlanApprovalDate;
            data.SingleAgendaMeeting = result.SingleAgendaMeeting;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;
            if(EvidenceofPlanApprovalResolution != null && Path.GetExtension(EvidenceofPlanApprovalResolution.FileName) == ".pdf"  &&
                EvidenceofPlanApprovalResolution.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.EvidenceofPlanApprovalResolution_path);
                var fileNameEvidenceofPlanApprovalResolution = data.GPCode.ToString() + "-" + "epar" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionEvidenceofPlanApprovalResolution = Path.GetExtension(EvidenceofPlanApprovalResolution.FileName);
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
                data.EvidenceofPlanApprovalResolution = fileNameEvidenceofPlanApprovalResolution + extensionEvidenceofPlanApprovalResolution;
                data.EvidenceofPlanApprovalResolution_path = filePathEvidenceofPlanApprovalResolution;
            }
            if (TotalMembersDeclaration_pdf != null && Path.GetExtension(TotalMembersDeclaration_pdf.FileName) == ".pdf" &&
    TotalMembersDeclaration_pdf.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.TotalMembersDeclaration_pdf_path);
                var fileNameTotalMembersDeclaration_pdf = data.GPCode.ToString() + "-" + "tmd" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionTotalMembersDeclaration_pdf = Path.GetExtension(TotalMembersDeclaration_pdf.FileName);
                // Define the upload folder
                string uploadPathTotalMembersDeclaration_pdf = Path.Combine(_environment.WebRootPath, "storages/apa_mc_1/TMD");
                // Generate the file path
                string filePathTotalMembersDeclaration_pdf = Path.Combine(uploadPathTotalMembersDeclaration_pdf,
                    fileNameTotalMembersDeclaration_pdf + extensionTotalMembersDeclaration_pdf);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathTotalMembersDeclaration_pdf))
                {
                    // The file is saved in a buffer before being processed
                    await TotalMembersDeclaration_pdf.CopyToAsync(stream);
                }
                data.TotalMembersDeclaration_pdf = fileNameTotalMembersDeclaration_pdf + extensionTotalMembersDeclaration_pdf;
                data.TotalMembersDeclaration_pdf_path = filePathTotalMembersDeclaration_pdf;
            }
            if (TotalMembersDeclaration_excel != null && TotalMembersDeclaration_excel.Length < MaxContentLength && Path.GetExtension(TotalMembersDeclaration_excel.FileName) == ".xlsx")
            {
                RemoveFileFromServer(data.TotalMembersDeclaration_excel_path);
                var fileNameTotalMembersDeclaration_excel = data.GPCode.ToString() + "-" + "tmd_excel" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionTotalMembersDeclaration_excel = Path.GetExtension(TotalMembersDeclaration_excel.FileName);
                // Define the upload folder
                string uploadPathTotalMembersDeclaration_excel = Path.Combine(_environment.WebRootPath, "storages/apa_mc_1/TMD_EXCEL");
                // Generate the file path
                string filePathTotalMembersDeclaration_excel = Path.Combine(uploadPathTotalMembersDeclaration_excel,
                    fileNameTotalMembersDeclaration_excel + extensionTotalMembersDeclaration_excel);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathTotalMembersDeclaration_excel))
                {
                    // The file is saved in a buffer before being processed
                    await TotalMembersDeclaration_excel.CopyToAsync(stream);
                }
                data.TotalMembersDeclaration_excel = fileNameTotalMembersDeclaration_excel + extensionTotalMembersDeclaration_excel;
                data.TotalMembersDeclaration_excel_path = filePathTotalMembersDeclaration_excel;
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Your Report Uploaded Successfully!";
            return RedirectToAction("APA_MC_1_State", "StateReport");
        }
        /**
         APA_MC_1_State_Edit isgpp.toshali1 isgpp.toshali2
         */
        
        public async Task<IActionResult> APA_MC_1_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var user = HttpContext.Session.GetString("UserInfo");
                var data = await _context.A_APA_MC_1s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo"); 
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_MC_1_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_MC_1_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        /**
         APA_MC_2_State_Edit
         */
        public async Task<IActionResult> APA_MC_2_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_MC_2s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_MC_2_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_MC_2_State_Edit(long  ID, A_APA_MC_2 result, IFormFile DeclarationStatusPhysicalCompletedActivities, IFormFile DeclarationPlan_Implementation, IFormFile EvidenceofCompletedActivity)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_MC_2s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            //Data Upload in Database
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.NoofIssuedWorkOrder = result.NoofIssuedWorkOrder;
            data.NoofPhysicallyCompletedActivities = result.NoofPhysicallyCompletedActivities;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (DeclarationStatusPhysicalCompletedActivities != null && Path.GetExtension(DeclarationStatusPhysicalCompletedActivities.FileName) == ".xlsx" &&
    DeclarationStatusPhysicalCompletedActivities.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.DeclarationStatusPhysicalCompletedActivities_Path);
                var fileNameDeclarationStatusPhysicalCompletedActivities = data.GPCode.ToString() + "-" + "dspca" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionDeclarationStatusPhysicalCompletedActivities = Path.GetExtension(DeclarationStatusPhysicalCompletedActivities.FileName);
                // Define the upload folder
                string uploadPathDeclarationStatusPhysicalCompletedActivities = Path.Combine(_environment.WebRootPath, "storages/apa_mc_2/DSPCA");
                // Generate the file path
                string filePathDeclarationStatusPhysicalCompletedActivities = Path.Combine(uploadPathDeclarationStatusPhysicalCompletedActivities,
                    fileNameDeclarationStatusPhysicalCompletedActivities + extensionDeclarationStatusPhysicalCompletedActivities);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathDeclarationStatusPhysicalCompletedActivities))
                {
                    // The file is saved in a buffer before being processed
                    await DeclarationStatusPhysicalCompletedActivities.CopyToAsync(stream);
                }
                data.DeclarationStatusPhysicalCompletedActivities = fileNameDeclarationStatusPhysicalCompletedActivities + extensionDeclarationStatusPhysicalCompletedActivities;
                data.DeclarationStatusPhysicalCompletedActivities_Path = filePathDeclarationStatusPhysicalCompletedActivities;
            }
            if (EvidenceofCompletedActivity != null && Path.GetExtension(EvidenceofCompletedActivity.FileName) == ".pdf" &&
    EvidenceofCompletedActivity.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.EvidenceofCompletedActivity_Path);
                var fileNameEvidenceofCompletedActivity = data.GPCode.ToString() + "-" + "eoca" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionEvidenceofCompletedActivity = Path.GetExtension(EvidenceofCompletedActivity.FileName);
                // Define the upload folder
                string uploadPathEvidenceofCompletedActivity = Path.Combine(_environment.WebRootPath, "storages/apa_mc_2/EOCA");
                // Generate the file path
                string filePathEvidenceofCompletedActivity = Path.Combine(uploadPathEvidenceofCompletedActivity,
                    fileNameEvidenceofCompletedActivity + extensionEvidenceofCompletedActivity);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathEvidenceofCompletedActivity))
                {
                    // The file is saved in a buffer before being processed
                    await EvidenceofCompletedActivity.CopyToAsync(stream);
                }
                data.EvidenceofCompletedActivity = fileNameEvidenceofCompletedActivity + extensionEvidenceofCompletedActivity;
                data.EvidenceofCompletedActivity_Path = filePathEvidenceofCompletedActivity;
            }
            if (DeclarationPlan_Implementation != null && Path.GetExtension(DeclarationPlan_Implementation.FileName) == ".pdf" &&
    DeclarationPlan_Implementation.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.DeclarationPlan_Implementation_Path);
                var fileNameDeclarationPlan_Implementation = data.GPCode.ToString() + "-" + "dpi" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionDeclarationPlan_Implementation = Path.GetExtension(DeclarationPlan_Implementation.FileName);
                // Define the upload folder
                string uploadPathDeclarationPlan_Implementation = Path.Combine(_environment.WebRootPath, "storages/apa_mc_2/DPI");
                // Generate the file path
                string filePathDeclarationPlan_Implementation = Path.Combine(uploadPathDeclarationPlan_Implementation,
                    fileNameDeclarationPlan_Implementation + extensionDeclarationPlan_Implementation);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathDeclarationPlan_Implementation))
                {
                    // The file is saved in a buffer before being processed
                    await DeclarationPlan_Implementation.CopyToAsync(stream);
                }
                data.DeclarationPlan_Implementation = fileNameDeclarationPlan_Implementation + extensionDeclarationPlan_Implementation;
                data.DeclarationPlan_Implementation_Path = filePathDeclarationPlan_Implementation;
            }
            
            _context.SaveChanges();
            TempData["Success"] = "Your Report Uploaded Successfully!";
            return RedirectToAction("APA_MC_2_State", "StateReport");
        }
        /**
         APA_MC_2_State_Edit
         */
        public async Task<IActionResult> APA_MC_2_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_MC_2s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_MC_2_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_MC_2_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_MC_5_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_MC_5s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_MC_5_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_MC_5_State_Edit(long ID, A_APA_MC_5 result, IFormFile AuditCertificate)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_MC_5s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            if(result.FYCode == 0)
            {
                data.FYCode = data.FYCode;
            }
            else
            {
                data.FYCode = result.FYCode;
            }
                data.AuditOpinion = result.AuditOpinion;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;
            if (AuditCertificate != null && Path.GetExtension(AuditCertificate.FileName) == ".pdf" && AuditCertificate.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.AuditCertificate_Path);
                var fileNameAuditCertificate = data.GPCode.ToString() + "-" + "audit" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionAuditCertificate = Path.GetExtension(AuditCertificate.FileName);
                // Define the upload folder
                string uploadPathAuditCertificate = Path.Combine(_environment.WebRootPath, "storages/apa_mc_5/AUDIT");
                // Generate the file path
                string filePathAuditCertificate = Path.Combine(uploadPathAuditCertificate,
                    fileNameAuditCertificate + extensionAuditCertificate);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathAuditCertificate))
                {
                    // The file is saved in a buffer before being processed
                    await AuditCertificate.CopyToAsync(stream);
                }
                data.AuditCertificate = fileNameAuditCertificate + extensionAuditCertificate;
                data.AuditCertificate_Path = filePathAuditCertificate;
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Your Report Uploaded Successfully!";
            return RedirectToAction("APA_MC_5_State", "StateReport");
        }
        public async Task<IActionResult> APA_MC_5_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_MC_5s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_MC_5_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_MC_5_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_MC_6_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_MC_6s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_MC_6_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_MC_6_State_Edit(long ID, A_APA_MC_6 result, IFormFile OSRDataUpload,
            IFormFile CCERUpload_2024_2025)
        {
            int MaxContentLength = 1024 * 1024 * 6;
            var data = _context.A_APA_MC_6s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = 2425;

            data.TotalTaxinINR_2324 = result.TotalTaxinINR_2324;
            data.TotalTaxinINR_2425 = result.TotalTaxinINR_2425;

            data.TotalNonTaxinINR_2324 = result.TotalNonTaxinINR_2324;
            data.TotalNonTaxinINR_2425 = result.TotalNonTaxinINR_2425;

            data.TotalOSRinINR_2324 = result.TotalOSRinINR_2324;

            //data.TotalOSRinINR_2425 = result.TotalOSRinINR_2425;
            data.TotalOSRinINR_2425 = result.TotalTaxinINR_2425 + result.TotalNonTaxinINR_2425;
            data.OSRDeductionAmountinINR_2324 = result.OSRDeductionAmountinINR_2324;
            data.OSRDeductionAmountinINR_2425 = result.OSRDeductionAmountinINR_2425;

            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (OSRDataUpload != null && Path.GetExtension(OSRDataUpload.FileName) == ".xlsx" &&
    OSRDataUpload.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.OSRDataUpload_path);
                var fileNameOSRDataUpload = data.GPCode.ToString() + "-" + "osr2425" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionOSRDataUpload = Path.GetExtension(OSRDataUpload.FileName);
                // Define the upload folder
                string uploadPathOSRDataUpload = Path.Combine(_environment.WebRootPath, "storages/apa_mc_6/OSR");
                // Generate the file path
                string filePathOSRDataUpload = Path.Combine(uploadPathOSRDataUpload,
                    fileNameOSRDataUpload + extensionOSRDataUpload);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathOSRDataUpload))
                {
                    // The file is saved in a buffer before being processed
                    await OSRDataUpload.CopyToAsync(stream);
                }
                data.OSRDataUpload = fileNameOSRDataUpload + extensionOSRDataUpload;
                data.OSRDataUpload_path = filePathOSRDataUpload;
            }
            if (CCERUpload_2024_2025 != null && Path.GetExtension(CCERUpload_2024_2025.FileName) == ".pdf" &&
    CCERUpload_2024_2025.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.CCERUpload_2024_2025_path);
                var fileNameCCERUpload_2024_2025 = data.GPCode.ToString() + "-" + "ccer2425" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionCCERUpload_2024_2025 = Path.GetExtension(CCERUpload_2024_2025.FileName);
                // Define the upload folder
                string uploadPathCCERUpload_2024_2025 = Path.Combine(_environment.WebRootPath, "storages/apa_mc_6/CCEROSR/2425");
                // Generate the file path
                string filePathCCERUpload_2024_2025 = Path.Combine(uploadPathCCERUpload_2024_2025,
                    fileNameCCERUpload_2024_2025 + extensionCCERUpload_2024_2025);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathCCERUpload_2024_2025))
                {
                    // The file is saved in a buffer before being processed
                    await CCERUpload_2024_2025.CopyToAsync(stream);
                }
                data.CCERUpload_2024_2025 = fileNameCCERUpload_2024_2025 + extensionCCERUpload_2024_2025;
                data.CCERUpload_2024_2025_path = filePathCCERUpload_2024_2025;
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_MC_6_State", "StateReport");
            
        }
        public async Task<IActionResult> APA_MC_6_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_MC_6s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_MC_6_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_MC_6_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_1_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_1s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_1_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_1_State_Edit(long ID, A_APA_TE_1 result, IFormFile PlanActivitiesUploadForPDF, IFormFile PlanActivitiesUploadForExcel)
        {
            int MaxContentLength = 1024 * 1024 * 6;
            var data = _context.A_APA_TE_1s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.TotalNoofPlanActivities = result.TotalNoofPlanActivities;
            data.TotalNoofPlanUnderSankalpa = result.TotalNoofPlanUnderSankalpa;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (PlanActivitiesUploadForPDF != null && Path.GetExtension(PlanActivitiesUploadForPDF.FileName) == ".pdf" &&
    PlanActivitiesUploadForPDF.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.PlanActivitiesUploadForPDF_Path);
                var fileNamePlanActivitiesUploadForPDF = data.GPCode.ToString() + "-" + "anex4pdf" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionPlanActivitiesUploadForPDF = Path.GetExtension(PlanActivitiesUploadForPDF.FileName);
                // Define the upload folder
                string uploadPathPlanActivitiesUploadForPDF = Path.Combine(_environment.WebRootPath, "storages/apa_te_1/ANEXPDF");
                // Generate the file path
                string filePathPlanActivitiesUploadForPDF = Path.Combine(uploadPathPlanActivitiesUploadForPDF,
                    fileNamePlanActivitiesUploadForPDF + extensionPlanActivitiesUploadForPDF);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathPlanActivitiesUploadForPDF))
                {
                    // The file is saved in a buffer before being processed
                    await PlanActivitiesUploadForPDF.CopyToAsync(stream);
                }
                data.PlanActivitiesUploadForPDF = fileNamePlanActivitiesUploadForPDF + extensionPlanActivitiesUploadForPDF;
                data.PlanActivitiesUploadForPDF_Path = filePathPlanActivitiesUploadForPDF;
            }
            if (PlanActivitiesUploadForExcel != null && Path.GetExtension(PlanActivitiesUploadForExcel.FileName) == ".xlsx" &&
    PlanActivitiesUploadForExcel.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.PlanActivitiesUploadForExcel_Path);
                var fileNamePlanActivitiesUploadForExcel = data.GPCode.ToString() + "-" + "anex4xls" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionPlanActivitiesUploadForExcel = Path.GetExtension(PlanActivitiesUploadForExcel.FileName);
                // Define the upload folder
                string uploadPathPlanActivitiesUploadForExcel = Path.Combine(_environment.WebRootPath, "storages/apa_te_1/ANEXEXCEL");
                // Generate the file path
                string filePathPlanActivitiesUploadForExcel = Path.Combine(uploadPathPlanActivitiesUploadForExcel,
                    fileNamePlanActivitiesUploadForExcel + extensionPlanActivitiesUploadForExcel);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathPlanActivitiesUploadForExcel))
                {
                    // The file is saved in a buffer before being processed
                    await PlanActivitiesUploadForExcel.CopyToAsync(stream);
                }
                data.PlanActivitiesUploadForExcel = fileNamePlanActivitiesUploadForExcel + extensionPlanActivitiesUploadForExcel;
                data.PlanActivitiesUploadForExcel_Path = filePathPlanActivitiesUploadForExcel;
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_1_State", "StateReport");            
        }
        public async Task<IActionResult> APA_TE_1_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_1s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_1_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_1_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_2_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_2s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_2_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_2_State_Edit(long ID, A_APA_TE_2 result, IFormFile Form36Doc)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_2s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.TotalBudgetfor2526FY = result.TotalBudgetfor2526FY;
            data.TotalBudgetmore3_5Lakh = result.TotalBudgetmore3_5Lakh;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (Form36Doc != null && Path.GetExtension(Form36Doc.FileName) == ".xlsx" && Form36Doc.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Form36Doc_Path);
                var fileNameForm36Doc = data.GPCode.ToString() + "-" + "form36" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionForm36Doc = Path.GetExtension(Form36Doc.FileName);
                // Define the upload folder
                string uploadPathForm36Doc = Path.Combine(_environment.WebRootPath, "storages/apa_te_2/FORM36");
                // Generate the file path
                string filePathForm36Doc = Path.Combine(uploadPathForm36Doc,
                    fileNameForm36Doc + extensionForm36Doc);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathForm36Doc))
                {
                    // The file is saved in a buffer before being processed
                    await Form36Doc.CopyToAsync(stream);
                }
                data.Form36Doc = fileNameForm36Doc + extensionForm36Doc;
                data.Form36Doc_Path = filePathForm36Doc;
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_2_State", "StateReport");            
        }
        public async Task<IActionResult> APA_TE_2_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_2s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_2_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_2_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_3_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_3s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_MC_3_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_3_State_Edit(long ID, A_APA_TE_3 result,
            IFormFile MeetingResolutionUpload, IFormFile GPDeclarationUpload)
        {
            int MaxContentLength = 1024 * 1024 * 26;
            int MaxContentLength1 = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_3s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.TotalNoofMeetingsConducted = result.TotalNoofMeetingsConducted;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (MeetingResolutionUpload != null && Path.GetExtension(MeetingResolutionUpload.FileName) == ".pdf" &&
    MeetingResolutionUpload.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.MeetingResolutionUpload_path);
                var fileNameMeetingResolutionUpload = data.GPCode.ToString() + "-" + "mru" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionMeetingResolutionUpload = Path.GetExtension(MeetingResolutionUpload.FileName);
                // Define the upload folder
                string uploadPathMeetingResolutionUpload = Path.Combine(_environment.WebRootPath, "storages/apa_te_3/MRU");
                // Generate the file path
                string filePathMeetingResolutionUpload = Path.Combine(uploadPathMeetingResolutionUpload,
                    fileNameMeetingResolutionUpload + extensionMeetingResolutionUpload);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathMeetingResolutionUpload))
                {
                    // The file is saved in a buffer before being processed
                    await MeetingResolutionUpload.CopyToAsync(stream);
                }
                data.MeetingResolutionUpload = fileNameMeetingResolutionUpload + extensionMeetingResolutionUpload;
                data.MeetingResolutionUpload_path = filePathMeetingResolutionUpload;
            }
            if (GPDeclarationUpload != null && Path.GetExtension(GPDeclarationUpload.FileName) == ".pdf" &&
    GPDeclarationUpload.Length < MaxContentLength1)
            {
                RemoveFileFromServer(data.GPDeclarationUpload_path);
                var fileNameGPDeclarationUpload = data.GPCode.ToString() + "-" + "gdu" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionGPDeclarationUpload = Path.GetExtension(GPDeclarationUpload.FileName);
                // Define the upload folder
                string uploadPathGPDeclarationUpload = Path.Combine(_environment.WebRootPath, "storages/apa_te_3/GDU");
                // Generate the file path
                string filePathGPDeclarationUpload = Path.Combine(uploadPathGPDeclarationUpload,
                    fileNameGPDeclarationUpload + extensionGPDeclarationUpload);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathGPDeclarationUpload))
                {
                    // The file is saved in a buffer before being processed
                    await GPDeclarationUpload.CopyToAsync(stream);
                }
                data.GPDeclarationUpload = fileNameGPDeclarationUpload + extensionGPDeclarationUpload;
                data.GPDeclarationUpload_path = filePathGPDeclarationUpload;
            }
            _context.SaveChanges();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_3_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_3_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_3s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_3_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_3_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_4_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_4s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_MC_4_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_4_State_Edit(long ID, A_APA_TE_4 result, IFormFile GramSavaMeetingResolution, IFormFile DeclarationofAnnex6)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_4s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.TotalnoofActivitiesofGramSabha = result.TotalnoofActivitiesofGramSabha;
            data.TotalnoofActivitiesRecomendedByGramSabha = result.TotalnoofActivitiesRecomendedByGramSabha;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (GramSavaMeetingResolution != null && Path.GetExtension(GramSavaMeetingResolution.FileName) == ".pdf" &&
    GramSavaMeetingResolution.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.GramSavaMeetingResolution_Path);
                var fileNameGramSavaMeetingResolution = data.GPCode.ToString() + "-" + "gsmr" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionGramSavaMeetingResolution = Path.GetExtension(GramSavaMeetingResolution.FileName);
                // Define the upload folder
                string uploadPathGramSavaMeetingResolution = Path.Combine(_environment.WebRootPath, "storages/apa_te_4/GSMR");
                // Generate the file path
                string filePathGramSavaMeetingResolution = Path.Combine(uploadPathGramSavaMeetingResolution,
                    fileNameGramSavaMeetingResolution + extensionGramSavaMeetingResolution);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathGramSavaMeetingResolution))
                {
                    // The file is saved in a buffer before being processed
                    await GramSavaMeetingResolution.CopyToAsync(stream);
                }
                data.GramSavaMeetingResolution = fileNameGramSavaMeetingResolution + extensionGramSavaMeetingResolution;
                data.GramSavaMeetingResolution_Path = filePathGramSavaMeetingResolution;
            }
            if (DeclarationofAnnex6 != null && Path.GetExtension(DeclarationofAnnex6.FileName) == ".pdf" &&
    DeclarationofAnnex6.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.DeclarationofAnnex6_path);
                var fileNameDeclarationofAnnex6 = data.GPCode.ToString() + "-" + "doa6" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionDeclarationofAnnex6 = Path.GetExtension(DeclarationofAnnex6.FileName);
                // Define the upload folder
                string uploadPathDeclarationofAnnex6 = Path.Combine(_environment.WebRootPath, "storages/apa_te_4/DOA6");
                // Generate the file path
                string filePathDeclarationofAnnex6 = Path.Combine(uploadPathDeclarationofAnnex6,
                    fileNameDeclarationofAnnex6 + extensionDeclarationofAnnex6);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathDeclarationofAnnex6))
                {
                    // The file is saved in a buffer before being processed
                    await DeclarationofAnnex6.CopyToAsync(stream);
                }
                data.DeclarationofAnnex6 = fileNameDeclarationofAnnex6 + extensionDeclarationofAnnex6;
                data.DeclarationofAnnex6_path = filePathDeclarationofAnnex6;
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_4_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_4_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_4s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_4_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_4_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_5_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_5s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_5_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_5_State_Edit(long ID, A_APA_TE_5 result, 
            IFormFile Activity_1_DPR, IFormFile Activity_2_DPR,  
            IFormFile Activity_3_DPR,
            IFormFile Activity_4_DPR, 
            IFormFile Activity_5_DPR, 
            IFormFile CompletionCertificate, 
            IFormFile EntireWOHightingAll)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_5s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.ActivityName_1 = result.ActivityName_1;
            data.ActivityName_2 = result.ActivityName_2;
            data.ActivityName_3 = result.ActivityName_3;
            data.ActivityName_4 = result.ActivityName_4;
            data.ActivityName_5 = result.ActivityName_5;
            data.ActivityValue_1 = result.ActivityValue_1;
            data.ActivityValue_2 = result.ActivityValue_2;
            data.ActivityValue_3 = result.ActivityValue_3;
            data.ActivityValue_4 = result.ActivityValue_4;
            data.ActivityValue_5 = result.ActivityValue_5;

            data.FundofActivity_1 = result.FundofActivity_1;
            data.FundofActivity_2 = result.FundofActivity_2;
            data.FundofActivity_3 = result.FundofActivity_3;
            data.FundofActivity_4 = result.FundofActivity_4;
            data.FundofActivity_5 = result.FundofActivity_5;

            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_ID = HttpContext.Session.GetString("UserInfo").ToString();
            data.Entry_Time = DateTime.Now;

            if (Activity_1_DPR != null && Path.GetExtension(Activity_1_DPR.FileName) == ".pdf" &&
    Activity_1_DPR.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_1_DPR_Path);
                var fileNameActivity_1_DPR = data.GPCode.ToString() + "-" + "dpr_1" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_1_DPR = Path.GetExtension(Activity_1_DPR.FileName);
                // Define the upload folder
                string uploadPathActivity_1_DPR = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/DPR_1");
                // Generate the file path
                string filePathActivity_1_DPR = Path.Combine(uploadPathActivity_1_DPR,
                    fileNameActivity_1_DPR + extensionActivity_1_DPR);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_1_DPR))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_1_DPR.CopyToAsync(stream);
                }
                data.Activity_1_DPR = fileNameActivity_1_DPR + extensionActivity_1_DPR;
                data.Activity_1_DPR_Path = filePathActivity_1_DPR;
            }
            if (Activity_2_DPR != null && Path.GetExtension(Activity_2_DPR.FileName) == ".pdf" && Activity_2_DPR.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_2_DPR_Path);
                var fileNameActivity_2_DPR = data.GPCode.ToString() + "-" + "dpr_2" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_2_DPR = Path.GetExtension(Activity_2_DPR.FileName);
                // Define the upload folder
                string uploadPathActivity_2_DPR = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/DPR_2");
                // Generate the file path
                string filePathActivity_2_DPR = Path.Combine(uploadPathActivity_2_DPR,
                    fileNameActivity_2_DPR + extensionActivity_2_DPR);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_2_DPR))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_2_DPR.CopyToAsync(stream);
                }
                data.Activity_2_DPR = fileNameActivity_2_DPR + extensionActivity_2_DPR;
                data.Activity_2_DPR_Path = filePathActivity_2_DPR;
            }
            if (Activity_3_DPR != null && Path.GetExtension(Activity_3_DPR.FileName) == ".pdf" && Activity_3_DPR.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_3_DPR_Path);
                var fileNameActivity_3_DPR = data.GPCode.ToString() + "-" + "dpr_3" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_3_DPR = Path.GetExtension(Activity_3_DPR.FileName);
                // Define the upload folder
                string uploadPathActivity_3_DPR = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/DPR_3");
                // Generate the file path
                string filePathActivity_3_DPR = Path.Combine(uploadPathActivity_3_DPR,
                    fileNameActivity_3_DPR + extensionActivity_3_DPR);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_3_DPR))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_3_DPR.CopyToAsync(stream);
                }
                data.Activity_3_DPR = fileNameActivity_3_DPR + extensionActivity_3_DPR;
                data.Activity_3_DPR_Path = filePathActivity_3_DPR;
            }
            if (Activity_4_DPR != null && Path.GetExtension(Activity_4_DPR.FileName) == ".pdf" && Activity_4_DPR.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_4_DPR_Path);
                var fileNameActivity_4_DPR = data.GPCode.ToString() + "-" + "dpr_4" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_4_DPR = Path.GetExtension(Activity_4_DPR.FileName);
                // Define the upload folder
                string uploadPathActivity_4_DPR = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/DPR_4");
                // Generate the file path
                string filePathActivity_4_DPR = Path.Combine(uploadPathActivity_4_DPR,
                    fileNameActivity_4_DPR + extensionActivity_4_DPR);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_4_DPR))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_4_DPR.CopyToAsync(stream);
                }
                data.Activity_4_DPR = fileNameActivity_4_DPR + extensionActivity_4_DPR;
                data.Activity_4_DPR_Path = filePathActivity_4_DPR;
            }
            if (Activity_5_DPR != null && Path.GetExtension(Activity_5_DPR.FileName) == ".pdf" && Activity_5_DPR.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_5_DPR_Path);
                var fileNameActivity_5_DPR = data.GPCode.ToString() + "-" + "dpr_5" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_5_DPR = Path.GetExtension(Activity_5_DPR.FileName);
                // Define the upload folder
                string uploadPathActivity_5_DPR = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/DPR_5");
                // Generate the file path
                string filePathActivity_5_DPR = Path.Combine(uploadPathActivity_5_DPR,
                    fileNameActivity_5_DPR + extensionActivity_5_DPR);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_5_DPR))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_5_DPR.CopyToAsync(stream);
                }
                data.Activity_5_DPR = fileNameActivity_5_DPR + extensionActivity_5_DPR;
                data.Activity_5_DPR_Path = filePathActivity_5_DPR;
            }
            if (CompletionCertificate != null && Path.GetExtension(CompletionCertificate.FileName) == ".pdf" && CompletionCertificate.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.CompletionCertificate_Path);
                var fileNameCompletionCertificate = data.GPCode.ToString() + "-" + "cc" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionCompletionCertificate = Path.GetExtension(CompletionCertificate.FileName);
                // Define the upload folder
                string uploadPathCompletionCertificate = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/CC");
                // Generate the file path
                string filePathCompletionCertificate = Path.Combine(uploadPathCompletionCertificate,
                    fileNameCompletionCertificate + extensionCompletionCertificate);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathCompletionCertificate))
                {
                    // The file is saved in a buffer before being processed
                    await CompletionCertificate.CopyToAsync(stream);
                }
                data.CompletionCertificate = fileNameCompletionCertificate + extensionCompletionCertificate;
                data.CompletionCertificate_Path = filePathCompletionCertificate;
            }
            if (EntireWOHightingAll != null && Path.GetExtension(EntireWOHightingAll.FileName) == ".xlsx" && EntireWOHightingAll.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.EntireWOHightingAll_Path);
                var fileNameEntireWOHightingAll = data.GPCode.ToString() + "-" + "woha" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionEntireWOHightingAll = Path.GetExtension(EntireWOHightingAll.FileName);
                // Define the upload folder
                string uploadPathEntireWOHightingAll = Path.Combine(_environment.WebRootPath, "storages/apa_te_5/WOHA");
                // Generate the file path
                string filePathEntireWOHightingAll = Path.Combine(uploadPathEntireWOHightingAll,
                    fileNameEntireWOHightingAll + extensionEntireWOHightingAll);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathEntireWOHightingAll))
                {
                    // The file is saved in a buffer before being processed
                    await EntireWOHightingAll.CopyToAsync(stream);
                }
                data.EntireWOHightingAll = fileNameEntireWOHightingAll + extensionEntireWOHightingAll;
                data.EntireWOHightingAll_Path = filePathEntireWOHightingAll;
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_5_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_5_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_5s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_ID = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_5_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_5_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_6_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_6s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_6_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_6_State_Edit(long ID, A_APA_TE_6 result, IFormFile Activity_1_MT, IFormFile Activity_2_MT, IFormFile Activity_3_MT,
    IFormFile Activity_4_MT, IFormFile Activity_5_MT, IFormFile CompletionCertificate_MT, IFormFile EntireWOHightingAll_MT)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_6s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.ActivityName_1_MT = result.ActivityName_1_MT;
            data.ActivityName_2_MT = result.ActivityName_2_MT;
            data.ActivityName_3_MT = result.ActivityName_3_MT;
            data.ActivityName_4_MT = result.ActivityName_4_MT;
            data.ActivityName_5_MT = result.ActivityName_5_MT;
            data.ActivityValue_1_MT = result.ActivityValue_1_MT;
            data.ActivityValue_2_MT = result.ActivityValue_2_MT;
            data.ActivityValue_3_MT = result.ActivityValue_3_MT;
            data.ActivityValue_4_MT = result.ActivityValue_4_MT;
            data.ActivityValue_5_MT = result.ActivityValue_5_MT;

            data.FundofActivity_1_MT = result.FundofActivity_1_MT;
            data.FundofActivity_2_MT = result.FundofActivity_2_MT;
            data.FundofActivity_3_MT = result.FundofActivity_3_MT;
            data.FundofActivity_4_MT = result.FundofActivity_4_MT;
            data.FundofActivity_5_MT = result.FundofActivity_5_MT;

            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_ID = HttpContext.Session.GetString("UserInfo").ToString();
            data.Entry_Time = DateTime.Now;

            if (Activity_1_MT != null && Path.GetExtension(Activity_1_MT.FileName) == ".pdf" && Activity_1_MT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_1_MT_Path);
                var fileNameActivity_1_MT = data.GPCode.ToString() + "-" + "mt_1" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_1_MT = Path.GetExtension(Activity_1_MT.FileName);
                // Define the upload folder
                string uploadPathActivity_1_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/MT_1");
                // Generate the file path
                string filePathActivity_1_MT = Path.Combine(uploadPathActivity_1_MT,
                    fileNameActivity_1_MT + extensionActivity_1_MT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_1_MT))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_1_MT.CopyToAsync(stream);
                }
                data.Activity_1_MT = fileNameActivity_1_MT + extensionActivity_1_MT;
                data.Activity_1_MT_Path = filePathActivity_1_MT;
            }
            if (Activity_2_MT != null && Path.GetExtension(Activity_2_MT.FileName) == ".pdf" && Activity_2_MT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_2_MT_Path);
                var fileNameActivity_2_MT = data.GPCode.ToString() + "-" + "mt_2" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_2_MT = Path.GetExtension(Activity_2_MT.FileName);
                // Define the upload folder
                string uploadPathActivity_2_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/MT_2");
                // Generate the file path
                string filePathActivity_2_MT = Path.Combine(uploadPathActivity_2_MT,
                    fileNameActivity_2_MT + extensionActivity_2_MT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_2_MT))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_2_MT.CopyToAsync(stream);
                }
                data.Activity_2_MT = fileNameActivity_2_MT + extensionActivity_2_MT;
                data.Activity_2_MT_Path = filePathActivity_2_MT;
            }
            if (Activity_3_MT != null && Path.GetExtension(Activity_3_MT.FileName) == ".pdf" && Activity_3_MT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_3_MT_Path);
                var fileNameActivity_3_MT = data.GPCode.ToString() + "-" + "mt_3" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_3_MT = Path.GetExtension(Activity_3_MT.FileName);
                // Define the upload folder
                string uploadPathActivity_3_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/MT_3");
                // Generate the file path
                string filePathActivity_3_MT = Path.Combine(uploadPathActivity_3_MT,
                    fileNameActivity_3_MT + extensionActivity_3_MT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_3_MT))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_3_MT.CopyToAsync(stream);
                }
                data.Activity_3_MT = fileNameActivity_3_MT + extensionActivity_3_MT;
                data.Activity_3_MT_Path = filePathActivity_3_MT;
            }
            if (Activity_4_MT != null && Path.GetExtension(Activity_4_MT.FileName) == ".pdf" && Activity_4_MT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_4_MT_Path);
                var fileNameActivity_4_MT = data.GPCode.ToString() + "-" + "mt_4" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_4_MT = Path.GetExtension(Activity_4_MT.FileName);
                // Define the upload folder
                string uploadPathActivity_4_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/MT_4");
                // Generate the file path
                string filePathActivity_4_MT = Path.Combine(uploadPathActivity_4_MT,
                    fileNameActivity_4_MT + extensionActivity_4_MT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_4_MT))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_4_MT.CopyToAsync(stream);
                }
                data.Activity_4_MT = fileNameActivity_4_MT + extensionActivity_4_MT;
                data.Activity_4_MT_Path = filePathActivity_4_MT;
            }
            if (Activity_5_MT != null && Path.GetExtension(Activity_5_MT.FileName) == ".pdf" && Activity_5_MT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_5_MT_Path);
                var fileNameActivity_5_MT = data.GPCode.ToString() + "-" + "mt_5" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_5_MT = Path.GetExtension(Activity_5_MT.FileName);
                // Define the upload folder
                string uploadPathActivity_5_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/MT_5");
                // Generate the file path
                string filePathActivity_5_MT = Path.Combine(uploadPathActivity_5_MT,
                    fileNameActivity_5_MT + extensionActivity_5_MT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_5_MT))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_5_MT.CopyToAsync(stream);
                }
                data.Activity_5_MT = fileNameActivity_5_MT + extensionActivity_5_MT;
                data.Activity_5_MT_Path = filePathActivity_5_MT;
            }
            if (CompletionCertificate_MT != null && Path.GetExtension(CompletionCertificate_MT.FileName) == ".pdf" && CompletionCertificate_MT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.CompletionCertificate_MT_Path);
                var fileNameCompletionCertificate_MT = data.GPCode.ToString() + "-" + "ccmt" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionCompletionCertificate_MT = Path.GetExtension(CompletionCertificate_MT.FileName);
                // Define the upload folder
                string uploadPathCompletionCertificate_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/CCMT");
                // Generate the file path
                string filePathCompletionCertificate_MT = Path.Combine(uploadPathCompletionCertificate_MT,
                    fileNameCompletionCertificate_MT + extensionCompletionCertificate_MT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathCompletionCertificate_MT))
                {
                    // The file is saved in a buffer before being processed
                    await CompletionCertificate_MT.CopyToAsync(stream);
                }
                data.CompletionCertificate_MT = fileNameCompletionCertificate_MT + extensionCompletionCertificate_MT;
                data.CompletionCertificate_MT_Path = filePathCompletionCertificate_MT;
            }
            if (EntireWOHightingAll_MT != null && Path.GetExtension(EntireWOHightingAll_MT.FileName) == ".xlsx" && EntireWOHightingAll_MT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.EntireWOHightingAll_MT_Path);
                var fileNameEntireWOHightingAll_MT = data.GPCode.ToString() + "-" + "wohallmt" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionEntireWOHightingAll_MT = Path.GetExtension(EntireWOHightingAll_MT.FileName);
                // Define the upload folder
                string uploadPathEntireWOHightingAll_MT = Path.Combine(_environment.WebRootPath, "storages/apa_te_6/WOHAMT");
                // Generate the file path
                string filePathEntireWOHightingAll_MT = Path.Combine(uploadPathEntireWOHightingAll_MT,
                    fileNameEntireWOHightingAll_MT + extensionEntireWOHightingAll_MT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathEntireWOHightingAll_MT))
                {
                    // The file is saved in a buffer before being processed
                    await EntireWOHightingAll_MT.CopyToAsync(stream);
                }
                data.EntireWOHightingAll_MT = fileNameEntireWOHightingAll_MT + extensionEntireWOHightingAll_MT;
                data.EntireWOHightingAll_MT_Path = filePathEntireWOHightingAll_MT;
            }

            _context.SaveChanges();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_6_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_6_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_6s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_ID = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_6_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_6_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_7_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_7s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_7_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_7_State_Edit(long ID, A_APA_TE_7 result, IFormFile Activity_1_AQT, IFormFile Activity_2_AQT, IFormFile Activity_3_AQT,
    IFormFile Activity_4_AQT, IFormFile Activity_5_AQT, IFormFile CompletionCertificate_AQT, IFormFile EntireWOHightingAll_AQT)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_7s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.ActivityName_1_AQT = result.ActivityName_1_AQT;
            data.ActivityName_2_AQT = result.ActivityName_2_AQT;
            data.ActivityName_3_AQT = result.ActivityName_3_AQT;
            data.ActivityName_4_AQT = result.ActivityName_4_AQT;
            data.ActivityName_5_AQT = result.ActivityName_5_AQT;
            data.ActivityValue_1_AQT = result.ActivityValue_1_AQT;
            data.ActivityValue_2_AQT = result.ActivityValue_2_AQT;
            data.ActivityValue_3_AQT = result.ActivityValue_3_AQT;
            data.ActivityValue_4_AQT = result.ActivityValue_4_AQT;
            data.ActivityValue_5_AQT = result.ActivityValue_5_AQT;

            data.FundofActivity_1_AQT = result.FundofActivity_1_AQT;
            data.FundofActivity_2_AQT = result.FundofActivity_2_AQT;
            data.FundofActivity_3_AQT = result.FundofActivity_3_AQT;
            data.FundofActivity_4_AQT = result.FundofActivity_4_AQT;
            data.FundofActivity_5_AQT = result.FundofActivity_5_AQT;

            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_ID = HttpContext.Session.GetString("UserInfo").ToString();
            data.Entry_Time = DateTime.Now;

            if (Activity_1_AQT != null && Path.GetExtension(Activity_1_AQT.FileName) == ".pdf" && Activity_1_AQT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_1_AQT_Path);
                var fileNameActivity_1_AQT = data.GPCode.ToString() + "-" + "aqt_1" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_1_AQT = Path.GetExtension(Activity_1_AQT.FileName);
                // Define the upload folder
                string uploadPathActivity_1_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/AQT_1");
                // Generate the file path
                string filePathActivity_1_AQT = Path.Combine(uploadPathActivity_1_AQT,
                    fileNameActivity_1_AQT + extensionActivity_1_AQT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_1_AQT))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_1_AQT.CopyToAsync(stream);
                }
                data.Activity_1_AQT = fileNameActivity_1_AQT + extensionActivity_1_AQT;
                data.Activity_1_AQT_Path = filePathActivity_1_AQT;
            }
            if (Activity_2_AQT != null && Path.GetExtension(Activity_2_AQT.FileName) == ".pdf" && Activity_2_AQT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_2_AQT_Path);
                var fileNameActivity_2_AQT = data.GPCode.ToString() + "-" + "aqt_2" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_2_AQT = Path.GetExtension(Activity_2_AQT.FileName);
                // Define the upload folder
                string uploadPathActivity_2_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/AQT_2");
                // Generate the file path
                string filePathActivity_2_AQT = Path.Combine(uploadPathActivity_2_AQT,
                    fileNameActivity_2_AQT + extensionActivity_2_AQT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_2_AQT))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_2_AQT.CopyToAsync(stream);
                }
                data.Activity_2_AQT = fileNameActivity_2_AQT + extensionActivity_2_AQT;
                data.Activity_2_AQT_Path = filePathActivity_2_AQT;
            }
            if (Activity_3_AQT != null && Path.GetExtension(Activity_3_AQT.FileName) == ".pdf" && Activity_3_AQT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_3_AQT_Path);
                var fileNameActivity_3_AQT = data.GPCode.ToString() + "-" + "aqt_3" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_3_AQT = Path.GetExtension(Activity_3_AQT.FileName);
                // Define the upload folder
                string uploadPathActivity_3_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/AQT_3");
                // Generate the file path
                string filePathActivity_3_AQT = Path.Combine(uploadPathActivity_3_AQT,
                    fileNameActivity_3_AQT + extensionActivity_3_AQT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_3_AQT))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_3_AQT.CopyToAsync(stream);
                }
                data.Activity_3_AQT = fileNameActivity_3_AQT + extensionActivity_3_AQT;
                data.Activity_3_AQT_Path = filePathActivity_3_AQT;
            }
            if (Activity_4_AQT != null && Path.GetExtension(Activity_4_AQT.FileName) == ".pdf" && Activity_4_AQT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_4_AQT_Path);
                var fileNameActivity_4_AQT = data.GPCode.ToString() + "-" + "aqt_4" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_4_AQT = Path.GetExtension(Activity_4_AQT.FileName);
                // Define the upload folder
                string uploadPathActivity_4_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/AQT_4");
                // Generate the file path
                string filePathActivity_4_AQT = Path.Combine(uploadPathActivity_4_AQT,
                    fileNameActivity_4_AQT + extensionActivity_4_AQT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_4_AQT))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_4_AQT.CopyToAsync(stream);
                }
                data.Activity_4_AQT = fileNameActivity_4_AQT + extensionActivity_4_AQT;
                data.Activity_4_AQT_Path = filePathActivity_4_AQT;
            }
            if (Activity_5_AQT != null && Path.GetExtension(Activity_5_AQT.FileName) == ".pdf" && Activity_5_AQT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_5_AQT_Path);
                var fileNameActivity_5_AQT = data.GPCode.ToString() + "-" + "aqt_5" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_5_AQT = Path.GetExtension(Activity_5_AQT.FileName);
                // Define the upload folder
                string uploadPathActivity_5_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/AQT_5");
                // Generate the file path
                string filePathActivity_5_AQT = Path.Combine(uploadPathActivity_5_AQT,
                    fileNameActivity_5_AQT + extensionActivity_5_AQT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_5_AQT))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_5_AQT.CopyToAsync(stream);
                }
                data.Activity_5_AQT = fileNameActivity_5_AQT + extensionActivity_5_AQT;
                data.Activity_5_AQT_Path = filePathActivity_5_AQT;
            }
            if (CompletionCertificate_AQT != null && Path.GetExtension(CompletionCertificate_AQT.FileName) == ".pdf" && CompletionCertificate_AQT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.CompletionCertificate_AQT_Path);
                var fileNameCompletionCertificate_AQT = data.GPCode.ToString() + "-" + "ccaqt" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionCompletionCertificate_AQT = Path.GetExtension(CompletionCertificate_AQT.FileName);
                // Define the upload folder
                string uploadPathCompletionCertificate_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/CCAQT");
                // Generate the file path
                string filePathCompletionCertificate_AQT = Path.Combine(uploadPathCompletionCertificate_AQT,
                    fileNameCompletionCertificate_AQT + extensionCompletionCertificate_AQT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathCompletionCertificate_AQT))
                {
                    // The file is saved in a buffer before being processed
                    await CompletionCertificate_AQT.CopyToAsync(stream);
                }
                data.CompletionCertificate_AQT = fileNameCompletionCertificate_AQT + extensionCompletionCertificate_AQT;
                data.CompletionCertificate_AQT_Path = filePathCompletionCertificate_AQT;
            }
            if (EntireWOHightingAll_AQT != null && Path.GetExtension(EntireWOHightingAll_AQT.FileName) == ".xlsx" && EntireWOHightingAll_AQT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.EntireWOHightingAll_AQT_Path);
                var fileNameEntireWOHightingAll_AQT = data.GPCode.ToString() + "-" + "wohallaqt" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionEntireWOHightingAll_AQT = Path.GetExtension(EntireWOHightingAll_AQT.FileName);
                // Define the upload folder
                string uploadPathEntireWOHightingAll_AQT = Path.Combine(_environment.WebRootPath, "storages/apa_te_7/WOHAAQT");
                // Generate the file path
                string filePathEntireWOHightingAll_AQT = Path.Combine(uploadPathEntireWOHightingAll_AQT,
                    fileNameEntireWOHightingAll_AQT + extensionEntireWOHightingAll_AQT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathEntireWOHightingAll_AQT))
                {
                    // The file is saved in a buffer before being processed
                    await EntireWOHightingAll_AQT.CopyToAsync(stream);
                }
                data.EntireWOHightingAll_AQT = fileNameEntireWOHightingAll_AQT + extensionEntireWOHightingAll_AQT;
                data.EntireWOHightingAll_AQT_Path = filePathEntireWOHightingAll_AQT;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_7_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_7_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_7s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_ID = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_7_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_7_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_8_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_8s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_8_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_8_State_Edit(long ID, A_APA_TE_8 result, IFormFile Activity_1_WT)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_8s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.TotalNoofWTDone = result.TotalNoofWTDone;
            data.ActiveStatus = 5;
            data.User_ID = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (Activity_1_WT != null && Path.GetExtension(Activity_1_WT.FileName) == ".pdf" && Activity_1_WT.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_1_WT_Path);
                var fileNameActivity_1_WT = data.GPCode.ToString() + "-" + "wt" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_1_WT = Path.GetExtension(Activity_1_WT.FileName);
                // Define the upload folder
                string uploadPathActivity_1_WT = Path.Combine(_environment.WebRootPath, "storages/apa_te_8/WT");
                // Generate the file path
                string filePathActivity_1_WT = Path.Combine(uploadPathActivity_1_WT,
                    fileNameActivity_1_WT + extensionActivity_1_WT);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_1_WT))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_1_WT.CopyToAsync(stream);
                }
                data.Activity_1_WT = fileNameActivity_1_WT + extensionActivity_1_WT;
                data.Activity_1_WT_Path = filePathActivity_1_WT;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_8_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_8_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_8s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_ID = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_8_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_8_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_9_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_9s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_9_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_9_State_Edit(long ID, A_APA_TE_9 result, 
            IFormFile WOActivityActivitieshavingvaluesmorethanorequalstwolakh, 
            IFormFile InspectionReportActivitiesEstimate)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_9s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.TotalNoofInspectedActivitiesmorethanorequalstwolakh = result.TotalNoofInspectedActivitiesmorethanorequalstwolakh;
            data.TotalNoofActivitieshavingvaluesmorethanorequalstwolakh = result.TotalNoofActivitieshavingvaluesmorethanorequalstwolakh;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (WOActivityActivitieshavingvaluesmorethanorequalstwolakh != null && 
                Path.GetExtension(WOActivityActivitieshavingvaluesmorethanorequalstwolakh.FileName) == ".xlsx" && WOActivityActivitieshavingvaluesmorethanorequalstwolakh.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.WOActivityActivitieshavingvaluesmorethanorequalstwolakh_path);
                var fileNameWOActivityActivitieshavingvaluesmorethanorequalstwolakh = data.GPCode.ToString() + "-" + "woahv" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionWOActivityActivitieshavingvaluesmorethanorequalstwolakh = Path.GetExtension(WOActivityActivitieshavingvaluesmorethanorequalstwolakh.FileName);
                // Define the upload folder
                string uploadPathWOActivityActivitieshavingvaluesmorethanorequalstwolakh = Path.Combine(_environment.WebRootPath, "storages/apa_te_9/WOAHV");
                // Generate the file path
                string filePathWOActivityActivitieshavingvaluesmorethanorequalstwolakh = Path.Combine(uploadPathWOActivityActivitieshavingvaluesmorethanorequalstwolakh,
                    fileNameWOActivityActivitieshavingvaluesmorethanorequalstwolakh + extensionWOActivityActivitieshavingvaluesmorethanorequalstwolakh);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathWOActivityActivitieshavingvaluesmorethanorequalstwolakh))
                {
                    // The file is saved in a buffer before being processed
                    await WOActivityActivitieshavingvaluesmorethanorequalstwolakh.CopyToAsync(stream);
                }
                data.WOActivityActivitieshavingvaluesmorethanorequalstwolakh = fileNameWOActivityActivitieshavingvaluesmorethanorequalstwolakh + extensionWOActivityActivitieshavingvaluesmorethanorequalstwolakh;
                data.WOActivityActivitieshavingvaluesmorethanorequalstwolakh_path = filePathWOActivityActivitieshavingvaluesmorethanorequalstwolakh;
            }

            if (InspectionReportActivitiesEstimate != null && Path.GetExtension(InspectionReportActivitiesEstimate.FileName) == ".pdf" && InspectionReportActivitiesEstimate.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.InspectionReportActivitiesEstimate_path);
                var fileNameInspectionReportActivitiesEstimate = data.GPCode.ToString() + "-" + "irae" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionInspectionReportActivitiesEstimate = Path.GetExtension(InspectionReportActivitiesEstimate.FileName);
                // Define the upload folder
                string uploadPathInspectionReportActivitiesEstimate = Path.Combine(_environment.WebRootPath, "storages/apa_te_9/IRAE");
                // Generate the file path
                string filePathInspectionReportActivitiesEstimate = Path.Combine(uploadPathInspectionReportActivitiesEstimate,
                    fileNameInspectionReportActivitiesEstimate + extensionInspectionReportActivitiesEstimate);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathInspectionReportActivitiesEstimate))
                {
                    // The file is saved in a buffer before being processed
                    await InspectionReportActivitiesEstimate.CopyToAsync(stream);
                }
                data.InspectionReportActivitiesEstimate = fileNameInspectionReportActivitiesEstimate + extensionInspectionReportActivitiesEstimate;
                data.InspectionReportActivitiesEstimate_path = filePathInspectionReportActivitiesEstimate;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_9_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_9_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_9s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_9_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_9_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_10_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_10s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_10_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_10_State_Edit(long ID, A_APA_TE_10 result, IFormFile ResolutionDoconForm27)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_10s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.GBMeetingDate = result.GBMeetingDate;
            data.Form27Approved = result.Form27Approved;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (ResolutionDoconForm27 != null && Path.GetExtension(ResolutionDoconForm27.FileName) == ".pdf" && ResolutionDoconForm27.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.ResolutionDoconForm27_path);
                var fileNameResolutionDoconForm27 = data.GPCode.ToString() + "-" + "form27" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionResolutionDoconForm27 = Path.GetExtension(ResolutionDoconForm27.FileName);
                // Define the upload folder
                string uploadPathResolutionDoconForm27 = Path.Combine(_environment.WebRootPath, "storages/apa_te_10/FORM27");
                // Generate the file path
                string filePathResolutionDoconForm27 = Path.Combine(uploadPathResolutionDoconForm27,
                    fileNameResolutionDoconForm27 + extensionResolutionDoconForm27);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathResolutionDoconForm27))
                {
                    // The file is saved in a buffer before being processed
                    await ResolutionDoconForm27.CopyToAsync(stream);
                }
                data.ResolutionDoconForm27 = fileNameResolutionDoconForm27 + extensionResolutionDoconForm27;
                data.ResolutionDoconForm27_path = filePathResolutionDoconForm27;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_10_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_10_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_10s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_10_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_10_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_13_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_13s.FindAsync(ID);
                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_13_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_13_State_Edit(long ID, A_APA_TE_13 result, IFormFile Activity_1_CFCSFC, IFormFile Activity_2_CFCSFC, IFormFile Activity_3_CFCSFC,
   IFormFile Activity_4_CFCSFC, IFormFile Activity_5_CFCSFC, IFormFile EntireWOHightingAll_CFCSFC)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_13s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.ActivityName_1_CFCSFC = result.ActivityName_1_CFCSFC;
            data.ActivityName_2_CFCSFC = result.ActivityName_2_CFCSFC;
            data.ActivityName_3_CFCSFC = result.ActivityName_3_CFCSFC;
            data.ActivityName_4_CFCSFC = result.ActivityName_4_CFCSFC;
            data.ActivityName_5_CFCSFC = result.ActivityName_5_CFCSFC;
            data.ActivityValue_1_CFCSFC = result.ActivityValue_1_CFCSFC;
            data.ActivityValue_2_CFCSFC = result.ActivityValue_2_CFCSFC;
            data.ActivityValue_3_CFCSFC = result.ActivityValue_3_CFCSFC;
            data.ActivityValue_4_CFCSFC = result.ActivityValue_4_CFCSFC;
            data.ActivityValue_5_CFCSFC = result.ActivityValue_5_CFCSFC;

            data.FundofActivity_1_CFCSFC = result.FundofActivity_1_CFCSFC;
            data.FundofActivity_2_CFCSFC = result.FundofActivity_2_CFCSFC;
            data.FundofActivity_3_CFCSFC = result.FundofActivity_3_CFCSFC;
            data.FundofActivity_4_CFCSFC = result.FundofActivity_4_CFCSFC;
            data.FundofActivity_5_CFCSFC = result.FundofActivity_5_CFCSFC;

            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_ID = HttpContext.Session.GetString("UserInfo").ToString();
            data.Entry_Time = DateTime.Now;

            if (Activity_1_CFCSFC != null && Path.GetExtension(Activity_1_CFCSFC.FileName) == ".pdf" && Activity_1_CFCSFC.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_1_CFCSFC_Path);
                var fileNameActivity_1_CFCSFC = data.GPCode.ToString() + "-" + "cfcsfc_1" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_1_CFCSFC = Path.GetExtension(Activity_1_CFCSFC.FileName);
                // Define the upload folder
                string uploadPathActivity_1_CFCSFC = Path.Combine(_environment.WebRootPath, "storages/apa_te_13/CFCSFC_1");
                // Generate the file path
                string filePathActivity_1_CFCSFC = Path.Combine(uploadPathActivity_1_CFCSFC,
                    fileNameActivity_1_CFCSFC + extensionActivity_1_CFCSFC);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_1_CFCSFC))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_1_CFCSFC.CopyToAsync(stream);
                }
                data.Activity_1_CFCSFC = fileNameActivity_1_CFCSFC + extensionActivity_1_CFCSFC;
                data.Activity_1_CFCSFC_Path = filePathActivity_1_CFCSFC;
            }
            if (Activity_2_CFCSFC != null && Path.GetExtension(Activity_2_CFCSFC.FileName) == ".pdf" && Activity_2_CFCSFC.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_2_CFCSFC_Path);
                var fileNameActivity_2_CFCSFC = data.GPCode.ToString() + "-" + "cfcsfc_2" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_2_CFCSFC = Path.GetExtension(Activity_2_CFCSFC.FileName);
                // Define the upload folder
                string uploadPathActivity_2_CFCSFC = Path.Combine(_environment.WebRootPath, "storages/apa_te_13/CFCSFC_2");
                // Generate the file path
                string filePathActivity_2_CFCSFC = Path.Combine(uploadPathActivity_2_CFCSFC,
                    fileNameActivity_2_CFCSFC + extensionActivity_2_CFCSFC);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_2_CFCSFC))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_2_CFCSFC.CopyToAsync(stream);
                }
                data.Activity_2_CFCSFC = fileNameActivity_2_CFCSFC + extensionActivity_2_CFCSFC;
                data.Activity_2_CFCSFC_Path = filePathActivity_2_CFCSFC;
            }
            if (Activity_3_CFCSFC != null && Path.GetExtension(Activity_3_CFCSFC.FileName) == ".pdf" && Activity_3_CFCSFC.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_3_CFCSFC_Path);
                var fileNameActivity_3_CFCSFC = data.GPCode.ToString() + "-" + "cfcsfc_3" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_3_CFCSFC = Path.GetExtension(Activity_3_CFCSFC.FileName);
                // Define the upload folder
                string uploadPathActivity_3_CFCSFC = Path.Combine(_environment.WebRootPath, "storages/apa_te_13/CFCSFC_3");
                // Generate the file path
                string filePathActivity_3_CFCSFC = Path.Combine(uploadPathActivity_3_CFCSFC,
                    fileNameActivity_3_CFCSFC + extensionActivity_3_CFCSFC);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_3_CFCSFC))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_3_CFCSFC.CopyToAsync(stream);
                }
                data.Activity_3_CFCSFC = fileNameActivity_3_CFCSFC + extensionActivity_3_CFCSFC;
                data.Activity_3_CFCSFC_Path = filePathActivity_3_CFCSFC;
            }
            if (Activity_4_CFCSFC != null && Path.GetExtension(Activity_4_CFCSFC.FileName) == ".pdf" && Activity_4_CFCSFC.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_4_CFCSFC_Path);
                var fileNameActivity_4_CFCSFC = data.GPCode.ToString() + "-" + "cfcsfc_4" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_4_CFCSFC = Path.GetExtension(Activity_4_CFCSFC.FileName);
                // Define the upload folder
                string uploadPathActivity_4_CFCSFC = Path.Combine(_environment.WebRootPath, "storages/apa_te_13/CFCSFC_4");
                // Generate the file path
                string filePathActivity_4_CFCSFC = Path.Combine(uploadPathActivity_4_CFCSFC,
                    fileNameActivity_4_CFCSFC + extensionActivity_4_CFCSFC);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_4_CFCSFC))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_4_CFCSFC.CopyToAsync(stream);
                }
                data.Activity_4_CFCSFC = fileNameActivity_4_CFCSFC + extensionActivity_4_CFCSFC;
                data.Activity_4_CFCSFC_Path = filePathActivity_4_CFCSFC;
            }
            if (Activity_5_CFCSFC != null && Path.GetExtension(Activity_5_CFCSFC.FileName) == ".pdf" && Activity_5_CFCSFC.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Activity_5_CFCSFC_Path);
                var fileNameActivity_5_CFCSFC = data.GPCode.ToString() + "-" + "cfcsfc_5" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionActivity_5_CFCSFC = Path.GetExtension(Activity_5_CFCSFC.FileName);
                // Define the upload folder
                string uploadPathActivity_5_CFCSFC = Path.Combine(_environment.WebRootPath, "storages/apa_te_13/CFCSFC_5");
                // Generate the file path
                string filePathActivity_5_CFCSFC = Path.Combine(uploadPathActivity_5_CFCSFC,
                    fileNameActivity_5_CFCSFC + extensionActivity_5_CFCSFC);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathActivity_5_CFCSFC))
                {
                    // The file is saved in a buffer before being processed
                    await Activity_5_CFCSFC.CopyToAsync(stream);
                }
                data.Activity_5_CFCSFC = fileNameActivity_5_CFCSFC + extensionActivity_5_CFCSFC;
                data.Activity_5_CFCSFC_Path = filePathActivity_5_CFCSFC;
            }
            if (EntireWOHightingAll_CFCSFC != null && Path.GetExtension(EntireWOHightingAll_CFCSFC.FileName) == ".xlsx" && EntireWOHightingAll_CFCSFC.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.EntireWOHightingAll_CFCSFC_Path);
                var fileNameEntireWOHightingAll_CFCSFC = data.GPCode.ToString() + "-" + "wohall" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionEntireWOHightingAll_CFCSFC = Path.GetExtension(EntireWOHightingAll_CFCSFC.FileName);
                // Define the upload folder
                string uploadPathEntireWOHightingAll_CFCSFC = Path.Combine(_environment.WebRootPath, "storages/apa_te_13/WOHA");
                // Generate the file path
                string filePathEntireWOHightingAll_CFCSFC = Path.Combine(uploadPathEntireWOHightingAll_CFCSFC,
                    fileNameEntireWOHightingAll_CFCSFC + extensionEntireWOHightingAll_CFCSFC);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathEntireWOHightingAll_CFCSFC))
                {
                    // The file is saved in a buffer before being processed
                    await EntireWOHightingAll_CFCSFC.CopyToAsync(stream);
                }
                data.EntireWOHightingAll_CFCSFC = fileNameEntireWOHightingAll_CFCSFC + extensionEntireWOHightingAll_CFCSFC;
                data.EntireWOHightingAll_CFCSFC_Path = filePathEntireWOHightingAll_CFCSFC;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_13_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_13_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_13s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_ID = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_13_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_13_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_14_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_14s.FindAsync(ID);
                
                if (data != null)
                {
                    var mc6 = _context.A_APA_MC_6s.Where(q => q.GPCode == data.GPCode).FirstOrDefault();
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    ViewBag.Info = mc6.TotalTaxinINR_2425;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_14_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_14_State_Edit(long ID, A_APA_TE_14 result, IFormFile Form7Upload)
        {
            
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_14s.Where(q => q.ID == ID).FirstOrDefault();
            var mc6 = _context.A_APA_MC_6s.Where(q => q.GPCode == data.GPCode).FirstOrDefault();
            if (data == null) return NotFound();
            if (mc6 == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.Tax_non_tax_collected_ownsource_revenue = mc6.TotalTaxinINR_2425;
            data.TotalOSRDemandCount = result.TotalOSRDemandCount;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (Form7Upload != null && Path.GetExtension(Form7Upload.FileName) == ".pdf" && Form7Upload.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Form7Upload_path);
                var fileNameForm7Upload = data.GPCode.ToString() + "-" + "form7" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionForm7Upload = Path.GetExtension(Form7Upload.FileName);
                // Define the upload folder
                string uploadPathForm7Upload = Path.Combine(_environment.WebRootPath, "storages/apa_te_14/FORM7");
                // Generate the file path
                string filePathForm7Upload = Path.Combine(uploadPathForm7Upload,
                    fileNameForm7Upload + extensionForm7Upload);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathForm7Upload))
                {
                    // The file is saved in a buffer before being processed
                    await Form7Upload.CopyToAsync(stream);
                }
                data.Form7Upload = fileNameForm7Upload + extensionForm7Upload;
                data.Form7Upload_path = filePathForm7Upload;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_14_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_14_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_14s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_14_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_14_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_16_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_16s.FindAsync(ID);

                if (data != null)
                {
                    var apa6Data = _context.A_APA_MC_6s.Where(q => q.GPCode == data.GPCode).FirstOrDefault();
                    if (apa6Data != null)
                    {
                        ViewBag.Info = apa6Data.TotalOSRinINR_2425;
                    }
                    else
                    {
                        ViewBag.Info = 0;
                    }
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_16_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_16_State_Edit(long ID, A_APA_TE_16 result, IFormFile Upload_Annex_7)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_16s.Where(q => q.ID == ID).FirstOrDefault();            
            if (data == null) return NotFound();
            var mc6 = _context.A_APA_MC_6s.Where(q => q.GPCode == data.GPCode).FirstOrDefault();
            if (mc6 == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.OSR_Collected_202425 = mc6.TotalOSRinINR_2425;
            data.OSR_Utilized_Development = result.OSR_Utilized_Development;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (Upload_Annex_7 != null && Path.GetExtension(Upload_Annex_7.FileName) == ".xlsx" && Upload_Annex_7.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.Upload_Annex_7_Path);
                var fileNameUpload_Annex_7 = data.GPCode.ToString() + "-" + "annex7" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionUpload_Annex_7 = Path.GetExtension(Upload_Annex_7.FileName);
                // Define the upload folder
                string uploadPathUpload_Annex_7 = Path.Combine(_environment.WebRootPath, "storages/apa_te_16/ANNEX7");
                // Generate the file path
                string filePathUpload_Annex_7 = Path.Combine(uploadPathUpload_Annex_7,
                    fileNameUpload_Annex_7 + extensionUpload_Annex_7);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathUpload_Annex_7))
                {
                    // The file is saved in a buffer before being processed
                    await Upload_Annex_7.CopyToAsync(stream);
                }
                data.Upload_Annex_7 = fileNameUpload_Annex_7 + extensionUpload_Annex_7;
                data.Upload_Annex_7_Path = filePathUpload_Annex_7;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_16_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_16_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_16s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_16_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_16_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_17_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_17s.FindAsync(ID);

                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_17_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_17_State_Edit(long ID, A_APA_TE_17 result, IFormFile UploadMeetingResolutionConducted202425)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_17s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.TotalMeetingConducted202425 = result.TotalMeetingConducted202425;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (UploadMeetingResolutionConducted202425 != null && Path.GetExtension(UploadMeetingResolutionConducted202425.FileName) == ".pdf" && UploadMeetingResolutionConducted202425.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.UploadMeetingResolutionConducted202425_Path);
                var fileNameUploadMeetingResolutionConducted202425 = data.GPCode.ToString() + "-" + "mrc" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionUploadMeetingResolutionConducted202425 = Path.GetExtension(UploadMeetingResolutionConducted202425.FileName);
                // Define the upload folder
                string uploadPathUploadMeetingResolutionConducted202425 = Path.Combine(_environment.WebRootPath, "storages/apa_te_17/MRC");
                // Generate the file path
                string filePathUploadMeetingResolutionConducted202425 = Path.Combine(uploadPathUploadMeetingResolutionConducted202425,
                    fileNameUploadMeetingResolutionConducted202425 + extensionUploadMeetingResolutionConducted202425);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathUploadMeetingResolutionConducted202425))
                {
                    // The file is saved in a buffer before being processed
                    await UploadMeetingResolutionConducted202425.CopyToAsync(stream);
                }
                data.UploadMeetingResolutionConducted202425 = fileNameUploadMeetingResolutionConducted202425 + extensionUploadMeetingResolutionConducted202425;
                data.UploadMeetingResolutionConducted202425_Path = filePathUploadMeetingResolutionConducted202425;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_17_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_17_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_17s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_17_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_17_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_11_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_11s.FindAsync(ID);

                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_11_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_11_State_Edit(long ID, A_APA_TE_11 result, IFormFile ATRResolutionSubmissionDoc)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_11s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.ATRSubmissionDate = result.ATRSubmissionDate;
            data.LastAuditReportReceivedDate = result.LastAuditReportReceivedDate;
            data.MeetingDateonATR = result.MeetingDateonATR;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (ATRResolutionSubmissionDoc != null && Path.GetExtension(ATRResolutionSubmissionDoc.FileName) == ".pdf" && ATRResolutionSubmissionDoc.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.ATRResolutionSubmissionDoc_path);
                var fileNameATRResolutionSubmissionDoc = data.GPCode.ToString() + "-" + "atr" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionATRResolutionSubmissionDoc = Path.GetExtension(ATRResolutionSubmissionDoc.FileName);
                // Define the upload folder
                string uploadPathATRResolutionSubmissionDoc = Path.Combine(_environment.WebRootPath, "storages/apa_te_11/ATR");
                // Generate the file path
                string filePathATRResolutionSubmissionDoc = Path.Combine(uploadPathATRResolutionSubmissionDoc,
                    fileNameATRResolutionSubmissionDoc + extensionATRResolutionSubmissionDoc);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathATRResolutionSubmissionDoc))
                {
                    // The file is saved in a buffer before being processed
                    await ATRResolutionSubmissionDoc.CopyToAsync(stream);
                }
                data.ATRResolutionSubmissionDoc = fileNameATRResolutionSubmissionDoc + extensionATRResolutionSubmissionDoc;
                data.ATRResolutionSubmissionDoc_path = filePathATRResolutionSubmissionDoc;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_11_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_11_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_11s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_11_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_11_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_19_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_19s.FindAsync(ID);

                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_19_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_19_State_Edit(long ID,A_APA_TE_19 result, IFormFile UploadAnnex8_PDF, IFormFile UploadAnnex8_Excel)
        {
            int MaxContentLength = 1024 * 1024 * 11;
            var data = _context.A_APA_TE_19s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.ExpenditureforSFC = result.ExpenditureforSFC;
            data.ExpenditureforCFCUntied = result.ExpenditureforCFCUntied;
            data.ExpenditureforOSR = result.ExpenditureforOSR;
            data.UntiedAmountSpentfromCFCSFCOSR202425 = result.UntiedAmountSpentfromCFCSFCOSR202425;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (UploadAnnex8_PDF != null && Path.GetExtension(UploadAnnex8_PDF.FileName) == ".pdf" && UploadAnnex8_PDF.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.UploadAnnex8_PDF_path);
                var fileNameUploadAnnex8_PDF = data.GPCode.ToString() + "-" + "annexpdf" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionUploadAnnex8_PDF = Path.GetExtension(UploadAnnex8_PDF.FileName);
                // Define the upload folder
                string uploadPathUploadAnnex8_PDF = Path.Combine(_environment.WebRootPath, "storages/apa_te_19/ANNEXPDF");
                // Generate the file path
                string filePathUploadAnnex8_PDF = Path.Combine(uploadPathUploadAnnex8_PDF,
                    fileNameUploadAnnex8_PDF + extensionUploadAnnex8_PDF);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathUploadAnnex8_PDF))
                {
                    // The file is saved in a buffer before being processed
                    await UploadAnnex8_PDF.CopyToAsync(stream);
                }
                data.UploadAnnex8_PDF = fileNameUploadAnnex8_PDF + extensionUploadAnnex8_PDF;
                data.UploadAnnex8_PDF_path = filePathUploadAnnex8_PDF;
            }
            if (UploadAnnex8_Excel != null && Path.GetExtension(UploadAnnex8_Excel.FileName) == ".xlsx" && UploadAnnex8_Excel.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.UploadAnnex8_Excel_path);
                var fileNameUploadAnnex8_Excel = data.GPCode.ToString() + "-" + "annexexcel" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionUploadAnnex8_Excel = Path.GetExtension(UploadAnnex8_Excel.FileName);
                // Define the upload folder
                string uploadPathUploadAnnex8_Excel = Path.Combine(_environment.WebRootPath, "storages/apa_te_19/ANNEXEXCEL");
                // Generate the file path
                string filePathUploadAnnex8_Excel = Path.Combine(uploadPathUploadAnnex8_Excel,
                    fileNameUploadAnnex8_Excel + extensionUploadAnnex8_Excel);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathUploadAnnex8_Excel))
                {
                    // The file is saved in a buffer before being processed
                    await UploadAnnex8_Excel.CopyToAsync(stream);
                }
                data.UploadAnnex8_Excel = fileNameUploadAnnex8_Excel + extensionUploadAnnex8_Excel;
                data.UploadAnnex8_Excel_path = filePathUploadAnnex8_Excel;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_19_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_19_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_19s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_19_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_19_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        public async Task<IActionResult> APA_TE_20_State_Edit(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin")
            {
                var data = await _context.A_APA_TE_20s.FindAsync(ID);

                if (data != null)
                {
                    var fin = _context.mst_FinancialYears.Where(q => q.FYCode == data.FYCode).FirstOrDefault();
                    ViewBag.FYName = fin.FYName;
                    return View(data);
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_20_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> APA_TE_20_State_Edit(long ID, A_APA_TE_20 result, IFormFile GrievenceImage)
        {
            int MaxContentLength = 1024 * 1024 * 5;
            var data = _context.A_APA_TE_20s.Where(q => q.ID == ID).FirstOrDefault();
            if (data == null) return NotFound();
            data.GPCode = result.GPCode;
            data.FYCode = result.FYCode;
            data.GrievenceLogged = result.GrievenceLogged;
            data.GrievenceResolved = result.GrievenceResolved;
            data.ActiveStatus =  data.ActiveStatus + 1;
            data.User_Id = HttpContext.Session.GetString("UserInfo");
            data.Entry_Time = DateTime.Now;

            if (GrievenceImage != null && Path.GetExtension(GrievenceImage.FileName) == ".jpg" && GrievenceImage.Length < MaxContentLength)
            {
                RemoveFileFromServer(data.GrievenceImage_Path);
                var fileNameGrievenceImage = data.GPCode.ToString() + "-" + "griev" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var extensionGrievenceImage = Path.GetExtension(GrievenceImage.FileName);
                // Define the upload folder
                string uploadPathGrievenceImage = Path.Combine(_environment.WebRootPath, "storages/apa_te_20/Image");
                // Generate the file path
                string filePathGrievenceImage = Path.Combine(uploadPathGrievenceImage,
                    fileNameGrievenceImage + extensionGrievenceImage);
                //Using Buffering
                using (var stream = System.IO.File.Create(filePathGrievenceImage))
                {
                    // The file is saved in a buffer before being processed
                    await GrievenceImage.CopyToAsync(stream);
                }
                data.GrievenceImage = fileNameGrievenceImage + extensionGrievenceImage;
                data.GrievenceImage_Path = filePathGrievenceImage;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Your File is uploaded successfully!";
            return RedirectToAction("APA_TE_20_State", "StateReport");
        }
        public async Task<IActionResult> APA_TE_20_State_Action(long ID)
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "PMUAdmin" && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali1"
                && HttpContext.Session.GetString("UserInfo") != "isgpp.toshali2")
            {
                var data = await _context.A_APA_TE_20s.FindAsync(ID);
                if (data != null)
                {
                    data.ActiveStatus = 99;
                    data.User_Id = HttpContext.Session.GetString("UserInfo");
                    data.Entry_Time = DateTime.Now;
                    _context.SaveChanges();
                    return RedirectToAction("APA_TE_20_State", "StateReport");
                }
                else
                {
                    ViewBag.Failed = "Data of the GP not found!";
                    return RedirectToAction("APA_TE_20_State", "StateReport");
                }
            }
            else
            {
                TempData["Failed"] = "Your are not allowed";
                return RedirectToAction("Login", "User");
            }
        }
    }
}
