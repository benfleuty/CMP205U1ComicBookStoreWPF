using DundeeComicBookStore.Models;
using System.Windows;

namespace DundeeComicBookStore.Windows
{
    /// <summary>
    /// Interaction logic for StaffConfirmAction.xaml
    /// </summary>
    public partial class StaffConfirmAction : Window
    {
        public StaffConfirmAction()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void VerifyLoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = staffEmailTxt.Text.Trim();
            string password = staffPasswordPwb.Password;
            var staff = (StaffModel)DBAccessHelper.GetUser(email, password);
            // invalid staff memeber
            if (staff == null)
            {
                MessageBox.Show("Wrong email/password combination!");
                return;
            }
            // valid staff member
            DialogResult = true;
            Close();
        }
    }
}