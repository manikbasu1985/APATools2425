namespace APATools.Models.AdminModels
{
    public class AssignPMUUserforAPA
    {
        public string DistrictName { get; set; } = string.Empty;
        public long? Allowed { get; set; } 
        public long? DistrictCode { get; set; }
    }
}
