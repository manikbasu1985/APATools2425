namespace APATools.Models.ReportModels
{
    public class APA_TE_9_Report
    {
        public long ID { get; set; }
        public long GPCode { get; set; }
        public long FYCode { get; set; }
        public long TotalNoofActivitieshavingvaluesmorethanorequalstwolakh { get; set; }
        public long TotalNoofInspectedActivitiesmorethanorequalstwolakh { get; set; }

        public string WOActivityActivitieshavingvaluesmorethanorequalstwolakh { get; set; }


        public string WOActivityActivitieshavingvaluesmorethanorequalstwolakh_path { get; set; }


        public string InspectionReportActivitiesEstimate { get; set; }


        public string InspectionReportActivitiesEstimate_path { get; set; }

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
