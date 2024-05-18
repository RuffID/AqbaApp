namespace AqbaServer.Models.OkdeskPerformance
{
    public class GroupEmployee
    {
        public GroupEmployee()
        {
            Groups = [];
        }

        public int Id { get; set; }
        public int[] Groups { get; set; }
    }
}
