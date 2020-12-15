using DundeeComicBookStore.Pages;
using System.Windows;

namespace DundeeComicBookStore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow window = new MainWindow();
            LoginPage lp = new LoginPage();
            window.MainFrame.Content = lp;
            window.Show();
        }
    }
}