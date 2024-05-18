namespace AqbaApp.Interfaces
{
    public interface IOkdeskDictionary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }

        void Update(IOkdeskDictionary item);
        void UpdateWithoutChecked(IOkdeskDictionary item);
    }
}
