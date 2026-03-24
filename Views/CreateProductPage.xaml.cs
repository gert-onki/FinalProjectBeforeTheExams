using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using shittyEtsy.Data;
using shittyEtsy.Data.Models;
using shittyEtsy.Session;
using System;

namespace shittyEtsy.Views
{
    public sealed partial class CreateProductPage : Page
    {
        private readonly int _currentUserId;

        public CreateProductPage()
        {
            InitializeComponent();
            _currentUserId = SessionManager.CurrentUser.Id;
        }

        private void CreateProductButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text;
            string description = DescriptionInput.Text;
            string material = MaterialInput.Text;
            string productionTime = (ProductionTimeInput.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string complexity = (ComplexityInput.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string durability = (DurabilityInput.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string uniqueFeatures = UniqueFeaturesInput.Text;
            int categoryId = CategoryInput.SelectedIndex + 1; // maps to your category table

            // Validation
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
            {
                ShowError("Please fill in at least the product name and description.");
                return;
            }

            if (CategoryInput.SelectedItem == null)
            {
                ShowError("Please select a category.");
                return;
            }

            using var db = new AppDataContext();

            var product = new Products
            {
                UserId = _currentUserId,
                CatagoryId = categoryId,
                Name = name,
                Description = description,
                Material = material,
                ProductionTime = productionTime,
                Complexity = complexity,
                Durability = durability,
                UniqueFeatures = uniqueFeatures,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            db.Product.Add(product);
            db.SaveChanges();

            Frame.Navigate(typeof(HomePage));
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}