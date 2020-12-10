using DundeeComicBookStore.Interfaces;
using DundeeComicBookStore.Pages;
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
    /// Interaction logic for BasketItemViewer.xaml
    /// </summary>
    public partial class BasketItemViewerWindow : Window
    {
        private BasketPage caller;
        private IProduct product;

        private int quantity;

        public BasketItemViewerWindow(BasketPage caller, IProduct product, int quantity)
        {
            InitializeComponent();
            this.caller = caller;
            this.product = product;
            productTitleTextblock.Text = this.product.Name;
            productDesciptionTextblock.Text = this.product.Description;
            productStockCountTextblock.Text = this.product.UnitsInStock.ToString();
            quantityToBuy.Value = quantity;
            if (this.product.UnitsInStock > 1)
            {
                quantityToBuy.Maximum = (int)this.product.UnitsInStock;
                quantityToBuy.Minimum = 0;
            }
            else if (this.product.UnitsInStock == 1)
            {
                quantityToBuy.Minimum = 0;
                quantityToBuy.Maximum = 1;
            }
            else
            {
                productStockCountTextblock.Text = "OUT OF STOCK";
                quantityToBuy.Value = 0;

                quantityToBuy.IsEnabled = false;
                setQuantity.IsEnabled = false;

                tbQuantityInStock.Visibility =
                    tbQuantityToBuy.Visibility =
                    quantityToBuy.Visibility =
                    setQuantity.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseProductViewerButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void QuantityToBuy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            quantity = (int)quantityToBuy.Value;
        }

        private void setQuantity_Click(object sender, RoutedEventArgs e)
        {
            caller.UpdateBasket(product, quantity);
        }
    }
}