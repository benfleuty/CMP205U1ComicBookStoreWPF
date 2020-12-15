using DundeeComicBookStore.Models;
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
    /// Interaction logic for SalesViewerPage.xaml
    /// </summary>
    public partial class SalesViewerPage : BasePage
    {
        private StaffModel _staff;

        public StaffModel Staff
        {
            get { return _staff; }
            set { _staff = value; }
        }

        public SalesViewerPage(StaffModel staff)
        {
            InitializeComponent();
            Staff = staff;
        }
    }
}