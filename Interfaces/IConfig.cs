using Newtonsoft.Json;

namespace AqbaApp.Interfaces
{
    public interface IConfig
    {
        public Formatting Formatting { get; set; }
        public string PathToFolder { get; set; }
        public string PathToFile { get; set; }
    }
}
