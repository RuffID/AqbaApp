namespace AqbaApp.Interfaces
{
    public interface IOkdeskDictionary
    {
        long Id { get; set; }
        string Name { get; set; }
        bool IsChecked { get; set; }

        void Update(IOkdeskDictionary item);
        void UpdateWithoutChecked(IOkdeskDictionary item);
    }
}
