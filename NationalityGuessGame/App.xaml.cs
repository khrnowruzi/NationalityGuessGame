using NationalityGuessGame.ViewModel;
using System.Windows;

namespace NationalityGuessGame
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = new MainWindow(new MainViewModel());
            mainWindow.Show();
        }
    }
}
