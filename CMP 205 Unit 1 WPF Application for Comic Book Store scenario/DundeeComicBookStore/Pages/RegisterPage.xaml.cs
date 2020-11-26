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
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : BasePage
    {
        //private List<Image> ppList = new List<Image>();

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void PasswordPBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
        }

        private void PasswordConfirmPBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePageTo(new LoginPage());
        }
    }
}