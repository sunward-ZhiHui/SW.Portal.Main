namespace DocumentViewer.EntityModels
{
    public class SchedulerDisplay
    {
        public long? Id { get; set; }
        public string? Text { get; set; }
        public string? Color { get; set; }
        public long? PlanningForProductionProcessByMachineId {  get; set; }

    }
}
