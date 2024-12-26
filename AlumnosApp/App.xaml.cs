using AlumnosApp.Vistas;

namespace AlumnosApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new ListarAlumnos());
        }
    }
}