namespace AqbaApp.Model.OkdeskReport
{
    public class Issue
    {
        public long Id { get; set; }
        public long? PriorityId { get; set; }
        public long? StatusId { get; set; }
        public long? TypeId { get; set; }
    }
}
