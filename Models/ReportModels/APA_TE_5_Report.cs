using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace APATools.Models.ReportModels
{
    public class APA_TE_5_Report
    {
        public long ID { get; set; }
        public long GPCode { get; set; }
        public long FYCode { get; set; }

        [Column(TypeName = "text")]
        public string ActivityName_1 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ActivityValue_1 { get; set; }


        [Unicode(false)]
        public string Activity_1_DPR { get; set; }


        [Unicode(false)]
        public string Activity_1_DPR_Path { get; set; }


        [Column(TypeName = "text")]
        public string ActivityName_2 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ActivityValue_2 { get; set; }


        [Unicode(false)]
        public string Activity_2_DPR { get; set; }


        [Unicode(false)]
        public string Activity_2_DPR_Path { get; set; }


        [Column(TypeName = "text")]
        public string ActivityName_3 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ActivityValue_3 { get; set; }


        [Unicode(false)]
        public string Activity_3_DPR { get; set; }


        [Unicode(false)]
        public string Activity_3_DPR_Path { get; set; }


        [Column(TypeName = "text")]
        public string ActivityName_4 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ActivityValue_4 { get; set; }


        [Unicode(false)]
        public string Activity_4_DPR { get; set; }


        [Unicode(false)]
        public string Activity_4_DPR_Path { get; set; }


        [Column(TypeName = "text")]
        public string ActivityName_5 { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ActivityValue_5 { get; set; }


        [Unicode(false)]
        public string Activity_5_DPR { get; set; }


        [Unicode(false)]
        public string Activity_5_DPR_Path { get; set; }


        [Unicode(false)]
        public string EntireWOHightingAll { get; set; }


        [Unicode(false)]
        public string EntireWOHightingAll_Path { get; set; }


        [Unicode(false)]
        public string CompletionCertificate { get; set; }


        [Unicode(false)]
        public string CompletionCertificate_Path { get; set; }

        public string FundofActivity_1 { get; set; }
        public string FundofActivity_2 { get; set; }
        public string FundofActivity_3 { get; set; }
        public string FundofActivity_4 { get; set; }
        public string FundofActivity_5 { get; set; }
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
