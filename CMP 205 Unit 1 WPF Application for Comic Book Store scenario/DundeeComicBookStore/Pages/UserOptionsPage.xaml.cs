using DundeeComicBookStore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DundeeComicBookStore.Pages
{
    /// <summary>
    /// Interaction logic for UserOptionsPage.xaml
    /// </summary>
    public partial class UserOptionsPage : Page
    {
        private IUser _user;

        public IUser User
        { get { return _user; } set { _user = value; } }

        public UserOptionsPage(IUser loggedInUser)
        {
            InitializeComponent();
            User = loggedInUser;

            DisplayUserInfo();
        }

        private void DisplayUserInfo()
        {
            string message = $"Welcome, {User.FullName} ({User.EmailAddress})";
            usernameTextblock.Text = message;
        }
    }
}