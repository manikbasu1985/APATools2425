using System.ComponentModel.DataAnnotations;

namespace APATools.Models.ReportModels
{
    public class APA_TE_20_Report
    {
        public long ID { get; set; }
        public long GPCode { get; set; }
        public long FYCode { get; set; }
        public long GrievenceLogged { get; set; }

        public long GrievenceResolved { get; set; }
        public APA_TE_20_Report() { }
        public string GrievenceImage { get; set; }

        
        public string GrievenceImage_Path { get; set; }
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
