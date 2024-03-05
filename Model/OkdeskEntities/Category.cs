namespace AqbaApp.Model.OkdeskEntities
{
    public class Category : ViewModelBase
    {
        int id;
        string color;

        public int Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }

        public string Color { get { return color; } set { color = value; OnPropertyChanged(nameof(Color)); } }

        public Category() { }

        public Category(int id, string color)
        {
            Id = id;
            Color = color;
        }
    }
}
