using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using shittyEtsy.Data;
using shittyEtsy.Data.Models;
using shittyEtsy.Session;
using System;
using System.Collections.Generic;
using System.Linq;

namespace shittyEtsy.Views
{
    public sealed partial class ShoppingCartPage : Page
    {
        // Flat row for the ItemsControl
        private class CartRow
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public string Material { get; set; }
            public string PriceFormatted { get; set; }
        }

        public ShoppingCartPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Refresh();
        }

        private void Refresh()
        {
            var items = CartManager.Items;

            if (items.Count == 0)
            {
                EmptyState.Visibility = Visibility.Visible;
                CheckoutPanel.Visibility = Visibility.Collapsed;
                CartList.ItemsSource = null;
                return;
            }

            EmptyState.Visibility = Visibility.Collapsed;
            CheckoutPanel.Visibility = Visibility.Visible;

            CartList.ItemsSource = items.Select(p => new CartRow
            {
                ProductId = p.Id,
                Name = p.Name,
                Material = p.Material ?? string.Empty,
                PriceFormatted = $"€ {p.Price:0.00}"
            }).ToList();

            TotalText.Text = $"€ {CartManager.Total():0.00}";

            // Show current balance
            var balance = SessionManager.CurrentUser?.Balance ?? 0;
            BalanceText.Text = $"€ {balance:0.00}";
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productId)
            {
                CartManager.Remove(productId);
                Refresh();
            }
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var currentUser = SessionManager.CurrentUser;
            if (currentUser == null) return;

            var items = CartManager.Items.ToList();
            if (items.Count == 0) return;

            decimal total = CartManager.Total();

            using var db = new AppDataContext();

            // Re-fetch current user from DB so the balance is up to date
            var buyer = db.Users.Find(currentUser.Id);
            if (buyer == null) return;

            if (buyer.Balance < total)
            {
                FeedbackText.Text = $"Not enough balance. You need € {total:0.00} but have € {buyer.Balance:0.00}.";
                FeedbackText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(255, 255, 85, 85));
                FeedbackText.Visibility = Visibility.Visible;
                return;
            }

            // Create one order
            var order = new Orders
            {
                BuyerId = buyer.Id,
                TotalPrice = total,
                Status = "Paid",
                CreatedAt = DateTime.UtcNow
            };
            db.Add(order);
            db.SaveChanges(); // get the order ID

            // Create order items + transactions per seller
            foreach (var product in items)
            {
                db.Add(new OrderItems
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    MakerId = product.UserId,
                    Price = product.Price,
                    Status = "Pending",
                    StatusDescription = "Waiting for maker"
                });

                // Pay seller
                var seller = db.Users.Find(product.UserId);
                if (seller != null)
                {
                    seller.Balance = (seller.Balance ?? 0) + product.Price;

                    db.Add(new Transaction
                    {
                        FromUserId = buyer.Id,
                        ToUserId = product.UserId,
                        Amount = product.Price,
                        Type = "Purchase",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // Deduct from buyer
            buyer.Balance -= total;
            SessionManager.CurrentUser.Balance = buyer.Balance; // keep session in sync

            db.SaveChanges();

            CartManager.Clear();

            FeedbackText.Text = $"Order placed! € {total:0.00} paid. New balance: € {buyer.Balance:0.00}.";
            FeedbackText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                Microsoft.UI.ColorHelper.FromArgb(255, 76, 175, 80));
            FeedbackText.Visibility = Visibility.Visible;

            BuyButton.IsEnabled = false;
            Refresh();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HomePage));
        }
    }
}
