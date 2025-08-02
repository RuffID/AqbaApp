using AqbaApp.Interfaces;
using Newtonsoft.Json;

namespace AqbaApp.Model.OkdeskEntities
{
    public class KindParameter : IEntity
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
    }
}
