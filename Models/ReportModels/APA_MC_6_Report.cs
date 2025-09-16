using System.ComponentModel.DataAnnotations.Schema;

namespace APATools.Models.ReportModels
{
    public class APA_MC_6_Report
    {

        public string CCERUpload_2023_2024 { get; set; }

        public string CCERUpload_2024_2025 { get; set; }

        public string OSRDataUpload { get; set; }

        public string CCERUpload_2023_2024_path { get; set; }

        public string CCERUpload_2024_2025_path { get; set; }

        public string OSRDataUpload_path { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalTaxinINR_2324 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalNonTaxinINR_2324 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalOSRinINR_2324 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OSRDeductionAmountinINR_2324 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalTaxinINR_2425 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalNonTaxinINR_2425 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalOSRinINR_2425 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OSRDeductionAmountinINR_2425 { get; set; }
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
