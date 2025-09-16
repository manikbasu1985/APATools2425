namespace APATools.Models.ReportModels
{
    public class APA_MC_2_Report
    {
        public long NoofPhysicallyCompletedActivities { get; set; }
        public long NoofIssuedWorkOrder { get; set; }
        public string DeclarationStatusPhysicalCompletedActivities { get; set; }
        public string DeclarationStatusPhysicalCompletedActivities_Path { get; set; }
        public string DeclarationPlan_Implementation { get; set; }
        public string DeclarationPlan_Implementation_Path { get; set; }
        public string EvidenceofCompletedActivity { get; set; }
        public string EvidenceofCompletedActivity_Path { get; set; }
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
