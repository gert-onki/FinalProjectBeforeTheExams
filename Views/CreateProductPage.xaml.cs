using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using shittyEtsy.Data;
using shittyEtsy.Data.Models;
using shittyEtsy.Session;
using System;
using System.IO;
using System.Linq;
using Windows.Storage.Pickers;

namespace shittyEtsy.Views
{
    public sealed partial class CreateProductPage : Page
    {
        private readonly int _currentUserId;
        private byte[]? _imageData;

        public CreateProductPage()
        {
            InitializeComponent();
            _currentUserId = SessionManager.CurrentUser.Id;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadCategories();
        }

        private void LoadCategories()
        {
            using var db = new AppDataContext();
            var categories = db.Categories.ToList();

            CategoryInput.Items.Clear();
            foreach (var cat in categories)
                CategoryInput.Items.Add(new ComboBoxItem { Content = cat.Name, Tag = cat.Id });
        }

        private async void PickImageButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file == null) return;

            _imageData = File.ReadAllBytes(file.Path);
            var bitmap = new BitmapImage(new Uri(file.Path));
            ImagePreview.Source = bitmap;
            ImagePreview.Visibility = Visibility.Visible;
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
            decimal price = decimal.TryParse(PriceInput.Text, out var p) ? p : 0;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
            {
                ShowError("Please fill in at least the product name and description.");
                return;
            }

            if (CategoryInput.SelectedItem is not ComboBoxItem selectedCategory)
            {
                ShowError("Please select a category.");
                return;
            }

            int categoryId = (int)selectedCategory.Tag;

            using var db = new AppDataContext();
            var product = new Products
            {
                UserId = _currentUserId,
                CatagoryId = categoryId,
                Name = name,
                Description = description,
                Material = material,
                Price = price,
                ProductionTime = productionTime,
                Complexity = complexity,
                Durability = durability,
                UniqueFeatures = uniqueFeatures,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow,
                ImageData = _imageData
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HomePage));
        }
    }
}