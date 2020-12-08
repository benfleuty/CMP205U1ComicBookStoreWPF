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
using System.Windows.Shapes;

namespace DundeeComicBookStore.Windows
{
    /// <summary>
    /// Interaction logic for ProductViewer.xaml
    /// </summary>
    public partial class ProductViewerWindow : Window
    {
        private Window _caller;

        public ProductViewerWindow(Window caller, IProduct product)
        {
            InitializeComponent();
            _caller = caller;
            productTitleTextblock.Text = product.Name;
            productDesciptionTextblock.Text = product.Description;
            productStockCountTextblock.Text = product.UnitsInStock.ToString();
            quantityToAdd.Maximum = (int)product.UnitsInStock;
            quantityToAdd.Minimum = 1;
        }

        private void CloseProductViewerButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}