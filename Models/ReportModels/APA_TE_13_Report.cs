using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APATools.Models.ReportModels
{
    public class APA_TE_13_Report
    {
        public long ID { get; set; }
        public long GPCode { get; set; }
        public long FYCode { get; set; }

        [Column(TypeName = "text")]
        public string ActivityName_1_CFCSFC { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ActivityValue_1_CFCSFC { get; set; }


        [Unicode(false)]
        public string Activity_1_CFCSFC { get; set; }


        [Unicode(false)]
        public string Activity_1_CFCSFC_Path { get; set; }


        [Column(TypeName = "text")]
        public string ActivityName_2_CFCSFC { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ActivityValue_2_CFCSFC { get; set; }


        [Unicode(false)]
        public string Activity_2_CFCSFC { get; set; }


        [Unicode(false)]
        public string Activity_2_CFCSFC_Path { get; set; }


        [Column(TypeName = "text")]
        public string ActivityName_3_CFCSFC { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ActivityValue_3_CFCSFC { get; set; }


        [Unicode(false)]
        public string Activity_3_CFCSFC { get; set; }


        [Unicode(false)]
        public string Activity_3_CFCSFC_Path { get; set; }


        [Column(TypeName = "text")]
        public string ActivityName_4_CFCSFC { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ActivityValue_4_CFCSFC { get; set; }


        [Unicode(false)]
        public string Activity_4_CFCSFC { get; set; }


        [Unicode(false)]
        public string Activity_4_CFCSFC_Path { get; set; }


        [Column(TypeName = "text")]
        public string ActivityName_5_CFCSFC { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ActivityValue_5_CFCSFC { get; set; }


        [Unicode(false)]
        public string Activity_5_CFCSFC { get; set; }


        [Unicode(false)]
        public string Activity_5_CFCSFC_Path { get; set; }


        [Unicode(false)]
        public string EntireWOHightingAll_CFCSFC { get; set; }


        [Unicode(false)]
        public string EntireWOHightingAll_CFCSFC_Path { get; set; }
        
        [StringLength(50)]
        public string FundofActivity_1_CFCSFC { get; set; }
        
        [StringLength(50)]
        public string FundofActivity_2_CFCSFC { get; set; }
        
        [StringLength(50)]
        public string FundofActivity_3_CFCSFC { get; set; }
        
        [StringLength(50)]
        public string FundofActivity_4_CFCSFC { get; set; }
        
        [StringLength(50)]
        public string FundofActivity_5_CFCSFC { get; set; }
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
