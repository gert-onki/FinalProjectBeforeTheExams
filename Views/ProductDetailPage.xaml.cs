using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using shittyEtsy.Data;
using shittyEtsy.Data.Models;
using shittyEtsy.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace shittyEtsy.Views
{
    public sealed partial class ProductDetailPage : Page
    {
        private Products _product;

        // Simple view model for reviews
        private class ReviewRow
        {
            public string Stars { get; set; }
            public string BuyerName { get; set; }
            public string Comment { get; set; }
        }

        public ProductDetailPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is not int productId)
            {
                Frame.Navigate(typeof(HomePage));
                return;
            }

            UpdateCartCount();

            using var db = new AppDataContext();

            _product = db.Product.Find(productId);
            if (_product == null)
            {
                Frame.Navigate(typeof(HomePage));
                return;
            }

            // Fill basic info
            ProductName.Text = _product.Name;
            ProductPrice.Text = $"€ {_product.Price:0.00}";
            ProductDescription.Text = _product.Description;
            DetailMaterial.Text = _product.Material ?? "-";
            DetailProductionTime.Text = _product.ProductionTime ?? "-";
            DetailComplexity.Text = _product.Complexity ?? "-";
            DetailDurability.Text = _product.Durability ?? "-";

            if (!string.IsNullOrWhiteSpace(_product.UniqueFeatures))
                UniqueFeatures.Text = $"✦ {_product.UniqueFeatures}";

            // Seller name
            var seller = db.Users.Find(_product.UserId);
            SellerName.Text = seller != null ? $"By {seller.Name}" : string.Empty;

            // Image
            if (_product.ImageData != null && _product.ImageData.Length > 0)
            {
                var bitmap = new BitmapImage();
                using var ms = new MemoryStream(_product.ImageData);
                await bitmap.SetSourceAsync(ms.AsRandomAccessStream());
                ProductImage.Source = bitmap;
            }

            // Buttons — show Add to Cart only for products that aren't your own
            bool isOwn = SessionManager.CurrentUser != null &&
                         _product.UserId == SessionManager.CurrentUser.Id;

            if (isOwn)
            {
                EditButton.Visibility = Visibility.Visible;
            }
            else
            {
                AddToCartButton.Visibility = Visibility.Visible;
                if (CartManager.Contains(_product.Id))
                {
                    AddToCartButton.Content = "Already in cart";
                    AddToCartButton.IsEnabled = false;
                }
            }

            // Reviews
            var reviews = db.Reviews
                .Where(r => r.ProductId == productId)
                .ToList();

            if (reviews.Count == 0)
            {
                NoReviewsText.Visibility = Visibility.Visible;
            }
            else
            {
                var rows = new List<ReviewRow>();
                foreach (var review in reviews)
                {
                    var buyer = db.Users.Find(review.BuyerId);
                    rows.Add(new ReviewRow
                    {
                        Stars = new string('★', review.Rating) + new string('☆', 5 - review.Rating),
                        BuyerName = buyer?.Name ?? "Unknown",
                        Comment = review.Comment
                    });
                }
                ReviewsList.ItemsSource = rows;
            }
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            CartManager.Add(_product);
            AddToCartButton.Content = "Already in cart";
            AddToCartButton.IsEnabled = false;
            CartFeedback.Text = "Added to cart!";
            CartFeedback.Visibility = Visibility.Visible;
            UpdateCartCount();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditProductPage), _product.Id);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HomePage));
        }

        private void GoToCartButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShoppingCartPage));
        }

        private void UpdateCartCount()
        {
            CartCountText.Text = $"Cart ({CartManager.Count})";
        }
    }
}
