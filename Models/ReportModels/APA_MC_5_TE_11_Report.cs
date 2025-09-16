namespace APATools.Models.ReportModels
{
    public class APA_MC_5_TE_11_Report
    {
        public DateOnly LastAuditReportReceivedDate { get; set; }
        public string EvidenceofAuditReportReceived { get; set; }
        public string EvidenceofAuditReportReceived_path { get; set; }
        public DateOnly MeetingDateonATR { get; set; }
        public string AuditOpinion { get; set; }
        public string ATRResolutionSubmissionDoc { get; set; }
        public string ATRResolutionSubmissionDoc_path { get; set; }
        public DateOnly ATRSubmissionDate { get; set; }
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
