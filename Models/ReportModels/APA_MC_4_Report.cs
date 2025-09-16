using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APATools.Models.ReportModels
{
    public class APA_MC_4_Report
    {
        public double? LGDCODE { get; set; }

        public long? GPCode { get; set; }

        [StringLength(255)]
        public string DISTRICT { get; set; }

        [StringLength(255)]
        public string BLOCK { get; set; }

        [StringLength(255)]
        public string GP { get; set; }

        [StringLength(255)]
        public string FYName { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? APR_2024 { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? MAY_2024 { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? JUN_2024 { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? JUL_2024 { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? AUG_2024 { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? SEP_2024 { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? OCT_2024 { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NOV_2024 { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DEC_2024 { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? JAN_2025 { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? FEB_2025 { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? MAR_2025 { get; set; }

        public double? Total { get; set; }

        [StringLength(255)]
        public string A_APA_MC_4_Result { get; set; }

        public long? FYCode { get; set; }
    }
}
