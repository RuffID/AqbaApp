using AqbaApp.Core;
using AqbaApp.Interfaces;

namespace AqbaApp.Model.OkdeskEntities
{
    public class Category : NotifyProperty, IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
}
