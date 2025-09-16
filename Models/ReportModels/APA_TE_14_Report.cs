using System.ComponentModel.DataAnnotations.Schema;

namespace APATools.Models.ReportModels
{
    public class APA_TE_14_Report
    {
        public long ID { get; set; }
        public long GPCode { get; set; }
        public long FYCode { get; set; }
        public string Form7Upload { get; set; }
        public string Form7Upload_path { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Tax_non_tax_collected_ownsource_revenue { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalOSRDemandCount { get; set; }

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
