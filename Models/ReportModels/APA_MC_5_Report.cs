namespace APATools.Models.ReportModels
{
    public class APA_MC_5_Report
    {

        public string AuditOpinion { get; set; }
        public string AuditCertificate { get; set; } = string.Empty;
        public string AuditCertificate_Path { get; set; } = string.Empty;
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
    }
}
