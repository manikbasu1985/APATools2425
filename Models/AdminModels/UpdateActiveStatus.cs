using System.ComponentModel.DataAnnotations;

namespace APATools.Models.AdminModels
{
    public class UpdateActiveStatus
    {
        public long? DistrictCode { get; set; }
        [StringLength(250)]
        public string DistrictName { get; set; }
        [StringLength(250)]
        public string TableName { get; set; }
    }
}
