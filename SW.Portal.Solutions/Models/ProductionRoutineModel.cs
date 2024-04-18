namespace SW.Portal.Solutions.Models
{
    public class ProductionRoutineModel
    {
        public long? ProductionActivityRoutineAppLineId { get; set; }
        public string Message { get; set; }
        public Guid? LineSessionId { get; set; }
    }
    public class DeleteIPIRAppModel
    {
        public long? IpirAppId { get; set; }
        public string Message { get; set; }
       
    }
}
