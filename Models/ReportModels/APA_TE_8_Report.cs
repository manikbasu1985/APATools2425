using Microsoft.EntityFrameworkCore;

namespace APATools.Models.ReportModels
{
    public class APA_TE_8_Report
    {
        public long ID { get; set; }
        public long GPCode { get; set; }
        public long FYCode { get; set; }
        public long TotalNoofWTDone { get; set; } = 0;
        [Unicode(false)]
        public string Activity_1_WT { get; set; }
        [Unicode(false)]
        public string Activity_1_WT_Path { get; set; }

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
