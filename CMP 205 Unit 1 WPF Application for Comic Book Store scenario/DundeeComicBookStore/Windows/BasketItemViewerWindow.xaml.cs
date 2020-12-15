using DundeeComicBookStore.Interfaces;
using DundeeComicBookStore.Pages;
using System.Windows;

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

        private void SetQuantity_Click(object sender, RoutedEventArgs e)
        {
            caller.UpdateBasket(product, quantity);
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            caller.CurrentOrder.Basket.Items.Remove(product);
            MessageBox.Show("This item was removed from your basket!", "Item removed");
            caller.OutputBasketItems();
            Close();
        }
    }
}