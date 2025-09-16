using System.ComponentModel.DataAnnotations.Schema;

namespace APATools.Models.ReportModels
{
    public class APA_TE_16_Report
    {
        public long ID { get; set; }
        public long GPCode { get; set; }
        public long FYCode { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal OSR_Collected_202425 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OSR_Utilized_Development { get; set; }
        public string Upload_Annex_7 { get; set; }
        public string Upload_Annex_7_Path { get; set; }

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
