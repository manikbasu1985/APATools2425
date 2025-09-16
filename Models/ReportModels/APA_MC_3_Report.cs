using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APATools.Models.ReportModels
{
    public class APA_MC_3_Report
    {
        public double? LGDCODE { get; set; }

        public long? GPCode { get; set; }

        [StringLength(255)]
        public string FYName { get; set; }

        [StringLength(255)]
        public string DISTRICT { get; set; }

        [StringLength(255)]
        public string BLOCK { get; set; }

        [StringLength(255)]
        public string GP { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? OB_CFC { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? OB_SFC { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? OB_OSR { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? RECPT_CFC_UPTO_DEC_2024 { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? RECPT_SFC_UPTO_DEC_2024 { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? RECPT_OSR_UPTO_DEC_2024 { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? EXP_CFC_UPTO_MAR_2025 { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? EXP_SFC_UPTO_MAR_2025 { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? EXP_OSR_UPTO_MAR_2025 { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Total_OB { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Total_Receipt { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Total_Expenditure { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Percentage { get; set; }

        [StringLength(255)]
        public string A_APA_MC_3_Result { get; set; }

        public long? FYCode { get; set; }

    }
}
