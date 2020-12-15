using DundeeComicBookStore.Interfaces;
using DundeeComicBookStore.Models;
using DundeeComicBookStore.Pages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            //string debugEmail = "bfleuty@outlook.com";
            string debugEmail = "bfleuty@outlook.com";
            string debugPassword = "123456";
            IUser user = DBAccessHelper.GetUser(debugEmail, debugPassword);

            MainWindow window = new MainWindow();
            if (user == null)
            {
                RegisterPage page = new RegisterPage();
                window.MainFrame.Content = page;
            }
            else
            {
                //OrderModel om = new OrderModel()
                //{
                //    User = user
                //};
                var staff = (StaffModel)user;
                var page = new StaffLandingPage(staff);
                window.MainFrame.Content = page;
            }

            window.Show();
        }
    }
}