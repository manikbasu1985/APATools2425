using APATools.Context;
using APATools.Controllers.District;
using APATools.Models.ReportModels;
using Microsoft.AspNetCore.Mvc;

namespace APATools.Controllers.Block
{
    public class BlockReportController : Controller
    {
        private readonly ILogger<BlockReportController> _logger;
        private readonly APAToolsContext _context;
        public BlockReportController(ILogger<BlockReportController> logger, APAToolsContext context)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult APA_MC_1_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_MC_1s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_MC_1_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               TotalNoofMember = ap.TotalNoofMember,
                               NoofMemberAttended = ap.NoofMemberAttended,
                               PlanApprovalDate = ap.PlanApprovalDate,
                               SingleAgendaMeeting = ap.SingleAgendaMeeting,
                               EvidenceofPlanApprovalResolution = ap.EvidenceofPlanApprovalResolution,
                               EvidenceofPlanApprovalResolution_path = ap.EvidenceofPlanApprovalResolution_path,
                               TotalMembersDeclaration_pdf = ap.TotalMembersDeclaration_pdf,
                               TotalMembersDeclaration_pdf_path = ap.TotalMembersDeclaration_pdf_path,
                               TotalMembersDeclaration_excel = ap.TotalMembersDeclaration_excel,
                               TotalMembersDeclaration_excel_path = ap.TotalMembersDeclaration_excel_path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_MC_2_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_MC_2s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_MC_2_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               DeclarationStatusPhysicalCompletedActivities = ap.DeclarationStatusPhysicalCompletedActivities,
                               DeclarationStatusPhysicalCompletedActivities_Path = ap.DeclarationStatusPhysicalCompletedActivities_Path,
                               NoofPhysicallyCompletedActivities = ap.NoofPhysicallyCompletedActivities,
                               NoofIssuedWorkOrder = ap.NoofIssuedWorkOrder,
                               DeclarationPlan_Implementation = ap.DeclarationPlan_Implementation,
                               DeclarationPlan_Implementation_Path = ap.DeclarationPlan_Implementation_Path,
                               EvidenceofCompletedActivity = ap.EvidenceofCompletedActivity,
                               EvidenceofCompletedActivity_Path = ap.EvidenceofCompletedActivity_Path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_MC_5_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_MC_5s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_MC_5_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               AuditOpinion = ap.AuditOpinion,
                               AuditCertificate = ap.AuditCertificate,
                               AuditCertificate_Path = ap.AuditCertificate_Path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_MC_6_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_MC_6s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_MC_6_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               TotalTaxinINR_2324 = ap.TotalTaxinINR_2324,
                               TotalNonTaxinINR_2324 = ap.TotalNonTaxinINR_2324,
                               TotalOSRinINR_2324 = ap.TotalOSRinINR_2324,
                               OSRDeductionAmountinINR_2324 = ap.OSRDeductionAmountinINR_2324,
                               TotalTaxinINR_2425 = ap.TotalTaxinINR_2425,
                               TotalNonTaxinINR_2425 = ap.TotalNonTaxinINR_2425,
                               TotalOSRinINR_2425 = ap.TotalOSRinINR_2425,
                               OSRDeductionAmountinINR_2425 = ap.OSRDeductionAmountinINR_2425,
                               OSRDataUpload = ap.OSRDataUpload,
                               OSRDataUpload_path = ap.OSRDataUpload_path,
                               CCERUpload_2024_2025 = ap.CCERUpload_2024_2025,
                               CCERUpload_2024_2025_path = ap.CCERUpload_2024_2025_path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_1_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_1s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_1_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               TotalNoofPlanActivities = ap.TotalNoofPlanActivities,
                               TotalNoofPlanUnderSankalpa = ap.TotalNoofPlanUnderSankalpa,
                               PlanActivitiesUploadForExcel = ap.PlanActivitiesUploadForExcel,
                               PlanActivitiesUploadForExcel_Path = ap.PlanActivitiesUploadForExcel_Path,
                               PlanActivitiesUploadForPDF = ap.PlanActivitiesUploadForPDF,
                               PlanActivitiesUploadForPDF_Path = ap.PlanActivitiesUploadForPDF_Path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_2_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_2s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_2_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               Form36Doc = ap.Form36Doc,
                               Form36Doc_Path = ap.Form36Doc_Path,
                               TotalBudgetfor2526FY = ap.TotalBudgetfor2526FY,
                               TotalBudgetmore3_5Lakh = ap.TotalBudgetmore3_5Lakh,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_3_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_3s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_3_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               GPDeclarationUpload = ap.GPDeclarationUpload,
                               GPDeclarationUpload_path = ap.GPDeclarationUpload_path,
                               MeetingResolutionUpload = ap.MeetingResolutionUpload,
                               MeetingResolutionUpload_path = ap.MeetingResolutionUpload_path,
                               TotalNoofMeetingsConducted = ap.TotalNoofMeetingsConducted,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_4_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_4s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_4_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               TotalnoofActivitiesofGramSabha = ap.TotalnoofActivitiesofGramSabha,
                               TotalnoofActivitiesRecomendedByGramSabha = ap.TotalnoofActivitiesRecomendedByGramSabha,
                               DeclarationofAnnex6 = ap.DeclarationofAnnex6,
                               DeclarationofAnnex6_path = ap.DeclarationofAnnex6_path,
                               GramSavaMeetingResolution = ap.GramSavaMeetingResolution,
                               GramSavaMeetingResolution_Path = ap.GramSavaMeetingResolution_Path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_5_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_5s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_5_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               ActivityName_1 = ap.ActivityName_1,
                               ActivityName_2 = ap.ActivityName_2,
                               ActivityName_3 = ap.ActivityName_3,
                               ActivityName_4 = ap.ActivityName_4,
                               ActivityName_5 = ap.ActivityName_5,
                               ActivityValue_1 = ap.ActivityValue_1,
                               ActivityValue_2 = ap.ActivityValue_2,
                               ActivityValue_3 = ap.ActivityValue_3,
                               ActivityValue_4 = ap.ActivityValue_4,
                               ActivityValue_5 = ap.ActivityValue_5,
                               Activity_1_DPR = ap.Activity_1_DPR,
                               Activity_1_DPR_Path = ap.Activity_1_DPR_Path,
                               Activity_2_DPR = ap.Activity_2_DPR,
                               Activity_2_DPR_Path = ap.Activity_2_DPR_Path,
                               Activity_3_DPR = ap.Activity_3_DPR,
                               Activity_3_DPR_Path = ap.Activity_3_DPR_Path,
                               Activity_4_DPR = ap.Activity_4_DPR,
                               Activity_4_DPR_Path = ap.Activity_4_DPR_Path,
                               Activity_5_DPR = ap.Activity_5_DPR,
                               Activity_5_DPR_Path = ap.Activity_5_DPR_Path,
                               CompletionCertificate = ap.CompletionCertificate,
                               CompletionCertificate_Path = ap.CompletionCertificate_Path,
                               EntireWOHightingAll = ap.EntireWOHightingAll,
                               EntireWOHightingAll_Path = ap.EntireWOHightingAll_Path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_6_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_6s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_6_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               ActivityName_1_MT = ap.ActivityName_1_MT,
                               ActivityName_2_MT = ap.ActivityName_2_MT,
                               ActivityName_3_MT = ap.ActivityName_3_MT,
                               ActivityName_4_MT = ap.ActivityName_4_MT,
                               ActivityName_5_MT = ap.ActivityName_5_MT,
                               ActivityValue_1_MT = ap.ActivityValue_1_MT,
                               ActivityValue_2_MT = ap.ActivityValue_2_MT,
                               ActivityValue_3_MT = ap.ActivityValue_3_MT,
                               ActivityValue_4_MT = ap.ActivityValue_4_MT,
                               ActivityValue_5_MT = ap.ActivityValue_5_MT,
                               Activity_1_MT = ap.Activity_1_MT,
                               Activity_1_MT_Path = ap.Activity_1_MT_Path,
                               Activity_2_MT = ap.Activity_2_MT,
                               Activity_2_MT_Path = ap.Activity_2_MT_Path,
                               Activity_3_MT = ap.Activity_3_MT,
                               Activity_3_MT_Path = ap.Activity_3_MT_Path,
                               Activity_4_MT = ap.Activity_4_MT,
                               Activity_4_MT_Path = ap.Activity_4_MT_Path,
                               Activity_5_MT = ap.Activity_5_MT,
                               Activity_5_MT_Path = ap.Activity_5_MT_Path,
                               CompletionCertificate_MT = ap.CompletionCertificate_MT,
                               CompletionCertificate_MT_Path = ap.CompletionCertificate_MT_Path,
                               EntireWOHightingAll_MT = ap.EntireWOHightingAll_MT,
                               EntireWOHightingAll_MT_Path = ap.EntireWOHightingAll_MT_Path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_7_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_7s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_7_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               ActivityName_1_AQT = ap.ActivityName_1_AQT,
                               ActivityName_2_AQT = ap.ActivityName_2_AQT,
                               ActivityName_3_AQT = ap.ActivityName_3_AQT,
                               ActivityName_4_AQT = ap.ActivityName_4_AQT,
                               ActivityName_5_AQT = ap.ActivityName_5_AQT,
                               ActivityValue_1_AQT = ap.ActivityValue_1_AQT,
                               ActivityValue_2_AQT = ap.ActivityValue_2_AQT,
                               ActivityValue_3_AQT = ap.ActivityValue_3_AQT,
                               ActivityValue_4_AQT = ap.ActivityValue_4_AQT,
                               ActivityValue_5_AQT = ap.ActivityValue_5_AQT,
                               Activity_1_AQT = ap.Activity_1_AQT,
                               Activity_1_AQT_Path = ap.Activity_1_AQT_Path,
                               Activity_2_AQT = ap.Activity_2_AQT,
                               Activity_2_AQT_Path = ap.Activity_2_AQT_Path,
                               Activity_3_AQT = ap.Activity_3_AQT,
                               Activity_3_AQT_Path = ap.Activity_3_AQT_Path,
                               Activity_4_AQT = ap.Activity_4_AQT,
                               Activity_4_AQT_Path = ap.Activity_4_AQT_Path,
                               Activity_5_AQT = ap.Activity_5_AQT,
                               Activity_5_AQT_Path = ap.Activity_5_AQT_Path,
                               CompletionCertificate_AQT = ap.CompletionCertificate_AQT,
                               CompletionCertificate_AQT_Path = ap.CompletionCertificate_AQT_Path,
                               EntireWOHightingAll_AQT = ap.EntireWOHightingAll_AQT,
                               EntireWOHightingAll_AQT_Path = ap.EntireWOHightingAll_AQT_Path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_8_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_8s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_8_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               TotalNoofWTDone = ap.TotalNoofWTDone,
                               Activity_1_WT = ap.Activity_1_WT,
                               Activity_1_WT_Path = ap.Activity_1_WT_Path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_9_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_9s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_9_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               TotalNoofInspectedActivitiesmorethanorequalstwolakh = ap.TotalNoofInspectedActivitiesmorethanorequalstwolakh,
                               InspectionReportActivitiesEstimate = ap.InspectionReportActivitiesEstimate,
                               InspectionReportActivitiesEstimate_path = ap.InspectionReportActivitiesEstimate_path,
                               TotalNoofActivitieshavingvaluesmorethanorequalstwolakh = ap.TotalNoofActivitieshavingvaluesmorethanorequalstwolakh,
                               WOActivityActivitieshavingvaluesmorethanorequalstwolakh = ap.WOActivityActivitieshavingvaluesmorethanorequalstwolakh,
                               WOActivityActivitieshavingvaluesmorethanorequalstwolakh_path = ap.WOActivityActivitieshavingvaluesmorethanorequalstwolakh_path,

                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_10_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_10s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_10_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               ResolutionDoconForm27 = ap.ResolutionDoconForm27,
                               ResolutionDoconForm27_path = ap.ResolutionDoconForm27_path,
                               GBMeetingDate = ap.GBMeetingDate,
                               Form27Approved = ap.Form27Approved,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_13_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_13s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_13_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               ActivityName_1_CFCSFC = ap.ActivityName_1_CFCSFC,
                               ActivityName_2_CFCSFC = ap.ActivityName_2_CFCSFC,
                               ActivityName_3_CFCSFC = ap.ActivityName_3_CFCSFC,
                               ActivityName_4_CFCSFC = ap.ActivityName_4_CFCSFC,
                               ActivityName_5_CFCSFC = ap.ActivityName_5_CFCSFC,
                               ActivityValue_1_CFCSFC = ap.ActivityValue_1_CFCSFC,
                               ActivityValue_2_CFCSFC = ap.ActivityValue_2_CFCSFC,
                               ActivityValue_3_CFCSFC = ap.ActivityValue_3_CFCSFC,
                               ActivityValue_4_CFCSFC = ap.ActivityValue_4_CFCSFC,
                               ActivityValue_5_CFCSFC = ap.ActivityValue_5_CFCSFC,
                               Activity_1_CFCSFC = ap.Activity_1_CFCSFC,
                               Activity_1_CFCSFC_Path = ap.Activity_1_CFCSFC_Path,
                               Activity_2_CFCSFC = ap.Activity_2_CFCSFC,
                               Activity_2_CFCSFC_Path = ap.Activity_2_CFCSFC_Path,
                               Activity_3_CFCSFC = ap.Activity_3_CFCSFC,
                               Activity_3_CFCSFC_Path = ap.Activity_3_CFCSFC_Path,
                               Activity_4_CFCSFC = ap.Activity_4_CFCSFC,
                               Activity_4_CFCSFC_Path = ap.Activity_4_CFCSFC_Path,
                               Activity_5_CFCSFC = ap.Activity_5_CFCSFC,
                               Activity_5_CFCSFC_Path = ap.Activity_5_CFCSFC_Path,
                               EntireWOHightingAll_CFCSFC = ap.EntireWOHightingAll_CFCSFC,
                               EntireWOHightingAll_CFCSFC_Path = ap.EntireWOHightingAll_CFCSFC_Path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_14_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_14s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_14_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               Form7Upload = ap.Form7Upload,
                               Form7Upload_path = ap.Form7Upload_path,
                               Tax_non_tax_collected_ownsource_revenue = ap.Tax_non_tax_collected_ownsource_revenue,
                               TotalOSRDemandCount = ap.TotalOSRDemandCount,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_16_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_16s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_16_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               OSR_Collected_202425 = ap.OSR_Collected_202425,
                               OSR_Utilized_Development = ap.OSR_Utilized_Development,
                               Upload_Annex_7 = ap.Upload_Annex_7,
                               Upload_Annex_7_Path = ap.Upload_Annex_7_Path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_17_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_17s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_17_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               TotalMeetingConducted202425 = ap.TotalMeetingConducted202425,
                               UploadMeetingResolutionConducted202425 = ap.UploadMeetingResolutionConducted202425,
                               UploadMeetingResolutionConducted202425_Path = ap.UploadMeetingResolutionConducted202425_Path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_19_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_19s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_19_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               ExpenditureforSFC = ap.ExpenditureforSFC,
                               ExpenditureforCFCUntied = ap.ExpenditureforCFCUntied,
                               ExpenditureforOSR = ap.ExpenditureforOSR,
                               UntiedAmountSpentfromCFCSFCOSR202425 = ap.UntiedAmountSpentfromCFCSFCOSR202425,
                               UploadAnnex8_Excel = ap.UploadAnnex8_Excel,
                               UploadAnnex8_Excel_path = ap.UploadAnnex8_Excel_path,
                               UploadAnnex8_PDF = ap.UploadAnnex8_PDF,
                               UploadAnnex8_PDF_path = ap.UploadAnnex8_PDF_path,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_MC_3_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_MC_3s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_MC_3_Report
                           {
                               DISTRICT = ap.DISTRICT,
                               BLOCK = ap.BLOCK,
                               GP = ap.GP,
                               FYCode = ap.FYCode,
                               FYName = ap.FYName,
                               OB_CFC = ap.OB_CFC,
                               OB_SFC = ap.OB_SFC,
                               OB_OSR = ap.OB_OSR,
                               RECPT_CFC_UPTO_DEC_2024 = ap.RECPT_CFC_UPTO_DEC_2024,
                               RECPT_SFC_UPTO_DEC_2024 = ap.RECPT_SFC_UPTO_DEC_2024,
                               RECPT_OSR_UPTO_DEC_2024 = ap.RECPT_OSR_UPTO_DEC_2024,
                               EXP_CFC_UPTO_MAR_2025 = ap.EXP_CFC_UPTO_MAR_2025,
                               EXP_SFC_UPTO_MAR_2025 = ap.EXP_SFC_UPTO_MAR_2025,
                               EXP_OSR_UPTO_MAR_2025 = ap.EXP_OSR_UPTO_MAR_2025,
                               Total_OB = ap.Total_OB,
                               Total_Expenditure = ap.Total_Expenditure,
                               Total_Receipt = ap.Total_Receipt,
                               Percentage = ap.Percentage,
                               A_APA_MC_3_Result = ap.A_APA_MC_3_Result
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_MC_4_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlocksAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_MC_4s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_MC_4_Report
                           {
                               DISTRICT = ap.DISTRICT,
                               BLOCK = ap.BLOCK,
                               GP = ap.GP,
                               GPCode = ap.GPCode,
                               APR_2024 = ap.APR_2024,
                               MAY_2024 = ap.MAY_2024,
                               JUN_2024 = ap.JUN_2024,
                               JUL_2024 = ap.JUL_2024,
                               AUG_2024 = ap.AUG_2024,
                               SEP_2024 = ap.SEP_2024,
                               OCT_2024 = ap.OCT_2024,
                               NOV_2024 = ap.NOV_2024,
                               DEC_2024 = ap.DEC_2024,
                               JAN_2025 = ap.JAN_2025,
                               FEB_2025 = ap.FEB_2025,
                               MAR_2025 = ap.MAR_2025,
                               Total = ap.Total,
                               A_APA_MC_4_Result = ap.A_APA_MC_4_Result
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_11_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_11s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_11_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               ATRResolutionSubmissionDoc = ap.ATRResolutionSubmissionDoc,
                               ATRResolutionSubmissionDoc_path = ap.ATRResolutionSubmissionDoc_path,
                               ATRSubmissionDate = ap.ATRSubmissionDate,
                               LastAuditReportReceivedDate = ap.LastAuditReportReceivedDate,
                               MeetingDateonATR = ap.MeetingDateonATR,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_18_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlocksAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_18s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_18_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               DCU_Reported_Expenditure_April_23_March_25 = ap.DCU_Reported_Expenditure_April_23_March_25,
                               System_Expenditure_April_23_March_25 = ap.System_Expenditure_April_23_March_25,
                               A_APA_TE_18_Result = ap.A_APA_TE_18_Result,
                               Deviation_Percentage = ap.Deviation_Percentage,
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
        public IActionResult APA_TE_20_Block()
        {
            if (HttpContext.Session.GetString("isLoggedIn") == "BlockAdmin")
            {
                var accessID = Convert.ToInt32(HttpContext.Session.GetString("UserAccessID"));
                var data = from ap in _context.A_APA_TE_20s
                           join loc in _context.view_alllocations on ap.GPCode equals loc.GPCode
                           join fin in _context.mst_FinancialYears on ap.FYCode equals fin.FYCode
                           where loc.BlockCode == accessID
                           select new APA_TE_20_Report
                           {
                               ID = ap.ID,
                               GPCode = ap.GPCode,
                               GPName = loc.GPName,
                               BlockName = loc.BlockName,
                               SubDivisionName = loc.SubDivisionName,
                               DistrictName = loc.DistrictName,
                               FYCode = ap.FYCode,
                               FYName = fin.FYName,
                               GrievenceResolved = ap.GrievenceResolved,
                               GrievenceLogged = ap.GrievenceLogged,
                               ActiveStatus = ap.ActiveStatus
                           };
                return View(data);
            }
            else
            {
                TempData["Failed"] = "You are not a valid user";
                return RedirectToAction("Login", "User");
            }
        }
    }
}
