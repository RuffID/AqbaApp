namespace AqbaApp.Model.OkdeskEntities
{
    public class Category : ViewModelBase
    {
        int id;
        string color;
        string name;
        string code;

        public int Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        public string Code { get { return code; } set { code = value; OnPropertyChanged(nameof(Code)); } }

        public string Color { get { return color; } set { color = value; OnPropertyChanged(nameof(Color)); } }

        public Category() { }

        public Category(int id, string color, string name, string code)
        {
            Id = id;
            Color = color;
            Name = name;
            Code = code;
        }
    }
}
