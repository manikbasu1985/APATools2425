namespace APATools.Models.ReportModels
{
    public class APA_TE_3_Report
    {
        public long ID { get; set; }
        public long GPCode { get; set; }
        public long FYCode { get; set; }
        public string MeetingResolutionUpload { get; set; }
        public string MeetingResolutionUpload_path { get; set; }
        public string GPDeclarationUpload { get; set; }
        public string GPDeclarationUpload_path { get; set; }

        public long TotalNoofMeetingsConducted { get; set; }

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
        public DateTime Entry_Time { get; set; }
    }
}
