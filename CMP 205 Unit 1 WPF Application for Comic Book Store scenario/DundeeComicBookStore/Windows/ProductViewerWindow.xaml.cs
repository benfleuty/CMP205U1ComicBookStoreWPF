using DundeeComicBookStore.Interfaces;
using DundeeComicBookStore.Pages;
using System.Windows;

namespace DundeeComicBookStore.Windows
{
    /// <summary>
    /// Interaction logic for ProductViewer.xaml
    /// </summary>
    public partial class ProductViewerWindow : Window
    {
        private SearchProductsPage caller;
        private IProduct product;

        public ProductViewerWindow(SearchProductsPage caller, IProduct product)
        {
            InitializeComponent();
            this.caller = caller;
            this.product = product;
            productTitleTextblock.Text = this.product.Name;
            productDesciptionTextblock.Text = this.product.Description;
            productStockCountTextblock.Text = this.product.UnitsInStock.ToString();
            if (this.product.UnitsInStock > 1)
            {
                quantityToAdd.Maximum = (int)this.product.UnitsInStock;
                quantityToAdd.Minimum = 1;
            }
            else if (this.product.UnitsInStock == 1)
                quantityToAdd.Minimum = quantityToAdd.Maximum = 1;
            else
            {
                productStockCountTextblock.Text = "OUT OF STOCK";
                quantityToAdd.Value = 0;

                quantityToAdd.IsEnabled = false;
                addQuantity.IsEnabled = false;

                tbQuantityInStock.Visibility =
                    tbQuantityToAdd.Visibility =
                    quantityToAdd.Visibility =
                    addQuantity.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseProductViewerButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddQuantity_Click(object sender, RoutedEventArgs e)
        {
            caller.CurrentOrder.Basket.Add(product, quantityToAdd.Value.GetValueOrDefault());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            caller.UpdateBasket();
        }
    }
}