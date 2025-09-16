using System.ComponentModel.DataAnnotations.Schema;

namespace APATools.Models.ReportModels
{
    public class APA_MC_1_Report
    {
        public long TotalNoofMember { get; set; }
        public long NoofMemberAttended { get; set; }
        public string EvidenceofPlanApprovalResolution { get; set; }

        public string EvidenceofPlanApprovalResolution_path { get; set; }
        public string TotalMembersDeclaration_pdf { get; set; }
        public string TotalMembersDeclaration_pdf_path { get; set; }
        public string TotalMembersDeclaration_excel { get; set; }
        public string TotalMembersDeclaration_excel_path { get; set; }
        public long ID { get; set; }
        public long GPCode { get; set; }
        public long FYCode { get; set; }

        public long DistrictCode { get; set; }
        public long SubDivisionCode { get; set; }
        public long BlockCode { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string SubDivisionName { get; set; }
        public string BlockName { get; set; }
        public string GPName { get; set; }
        public string FYName { get; set; }
        public long ActiveStatus { get; set; }
        public string User_Id { get; set; }
        public System.DateTime Entry_Time { get; set; }
        public string SingleAgendaMeeting { get; set; }
        [Column(TypeName = "date")]
        public DateTime PlanApprovalDate { get; set; }
    }
}
