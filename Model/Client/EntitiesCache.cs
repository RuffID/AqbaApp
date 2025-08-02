using AqbaApp.Model.OkdeskReport;
using System.Collections.Generic;

namespace AqbaApp.Model.Client
{
    public class EntitiesCache
    {
        public IList<Group> Groups { get; set; }

        public IList<Status> Statuses { get; set; }

        public IList<TaskType> Types { get; set; }

        public IList<Priority> Priorities { get; set; }

        public EntitiesCache()
        {
            Groups = [];
            Statuses = [];
            Types = [];
            Priorities = [];
        }
    }
}
