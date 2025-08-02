using AqbaApp.Model.Authorization;

namespace AqbaApp.Model.Client
{
    public class MainSettings
    {
        private int headerFontSize;
        private int fullNameFontSize;
        private int solvedTasksFontSize;
        private int spendedTimeFontSize;
        private int openTasksFontSize;

        public string ServerAddress { get; set; }
        public string PathToCLEARbat { get; set; }
        public string PathToAnydesk { get; set; }
        public string PathToAmmyAdmin { get; set; }
        public string PathToAssistant { get; set; }
        public string PathToRustDesk { get; set; }
        public string PathToBackground { get; set; }
        public int HeaderFontSize 
        { 
            get => headerFontSize; 
            set
            {
                if (value > 72 || value < 8)
                    headerFontSize = 18;
                else
                    headerFontSize = value;
            }
        }
        public int FullNameFontSize
        {
            get => fullNameFontSize;
            set
            {
                if (value > 72 || value < 8)
                    fullNameFontSize = 18;
                else
                    fullNameFontSize = value;
            }
        }
        public int SolvedTasksFontSize
        {
            get => solvedTasksFontSize;
            set
            {
                if (value > 72 || value < 8)
                    solvedTasksFontSize = 18;
                else
                    solvedTasksFontSize = value;
            }
        }
        public int SpendedTimeFontSize
        {
            get => spendedTimeFontSize;
            set
            {
                if (value > 72 || value < 8)
                    spendedTimeFontSize = 18;
                else
                    spendedTimeFontSize = value;
            }
        }
        public int OpenTasksFontSize
        {
            get => openTasksFontSize;
            set
            {
                if (value > 72 || value < 8)
                    openTasksFontSize = 18;
                else
                    openTasksFontSize = value;
            }
        }
        public string HeaderBackgroundColor { get; set; }
        public string FullNameBackgroundColor { get; set; }
        public string SpendedTimeBackgroundColor { get; set; }
        public string SolvedTasksBackgroundColor { get; set; }
        public string OpenTasksBackgroundColor { get; set; }        
        public bool ElectronicQueueMode { get; set; }
        public bool HideEmployeesWithoutSolvedIssues { get; set; }
        public bool HideEmployeesWithoutWrittenOffTime { get; set; }
        public bool HideEmployeesWithoutOpenIssues { get; set; }
        public AuthenticateResponse? Token { get; set; }

        public MainSettings()
        {
            ServerAddress = "http://localhost:8080";

            PathToCLEARbat = "C:\\iiko_Distr\\CLEAR.bat.exe";
            PathToAnydesk = "C:\\Program Files (x86)\\AnyDesk\\Anydesk.exe";
            PathToAmmyAdmin = "C:\\Program Files (x86)\\Ammy LLC\\Ammy Admin\\AA_v3.exe";
            PathToAssistant = "C:\\Program Files (x86)\\Ассистент\\assistant.exe";
            PathToRustDesk = "C:\\Program Files\\RustDesk\\rustdesk.exe";

            PathToBackground = "View/Resources/Background/White.png";
            HeaderFontSize = 18;
            FullNameFontSize = 16;
            SolvedTasksFontSize = 16;
            SpendedTimeFontSize = 16;
            OpenTasksFontSize = 16;

            HeaderBackgroundColor = "#000000";
            SolvedTasksBackgroundColor = "#000000";
            FullNameBackgroundColor = "#000000";
            SpendedTimeBackgroundColor = "#000000";
            OpenTasksBackgroundColor = "#000000";

            ElectronicQueueMode = false;
            HideEmployeesWithoutSolvedIssues = false;
            HideEmployeesWithoutOpenIssues = false;

            Token = new();
        }
    }
}
