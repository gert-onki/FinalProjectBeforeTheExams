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
    public sealed partial class EditProductPage : Page
    {
        private Products _product;
        private byte[]? _imageData;

        public EditProductPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is int productId)
            {
                LoadCategories();
                LoadProduct(productId);
            }
            else
            {
                ShowError("Product not found.");
                Frame.Navigate(typeof(HomePage));
            }
        }

        private void LoadCategories()
        {
            using var db = new AppDataContext();
            var categories = db.Categories.ToList();

            CategoryInput.Items.Clear();
            foreach (var cat in categories)
                CategoryInput.Items.Add(new ComboBoxItem { Content = cat.Name, Tag = cat.Id });
        }

        private void LoadProduct(int productId)
        {
            using var db = new AppDataContext();
            _product = db.Product.Find(productId);

            if (_product == null)
            {
                ShowError("Product not found.");
                Frame.Navigate(typeof(HomePage));
                return;
            }

            if (_product.UserId != SessionManager.CurrentUser.Id)
            {
                ShowError("You can only edit your own products.");
                Frame.Navigate(typeof(HomePage));
                return;
            }
            NameInput.Text = _product.Name;
            DescriptionInput.Text = _product.Description;
            MaterialInput.Text = _product.Material;
            PriceInput.Text = _product.Price.ToString("0.00");
            UniqueFeaturesInput.Text = _product.UniqueFeatures;

            for (int i = 0; i < CategoryInput.Items.Count; i++)
            {
                if (CategoryInput.Items[i] is ComboBoxItem item && (int)item.Tag == _product.CatagoryId)
                {
                    CategoryInput.SelectedIndex = i;
                    break;
                }
            }

            SetComboBoxByContent(ProductionTimeInput, _product.ProductionTime);
            SetComboBoxByContent(ComplexityInput, _product.Complexity);
            SetComboBoxByContent(DurabilityInput, _product.Durability);
            if (_product.ImageData != null && _product.ImageData.Length > 0)
            {
                _imageData = _product.ImageData;
                DisplayImage();
            }
        }

        private void SetComboBoxByContent(ComboBox comboBox, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return;

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i] is ComboBoxItem item && item.Content?.ToString() == content)
                {
                    comboBox.SelectedIndex = i;
                    break;
                }
            }
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
            DisplayImage();
        }

        private void DisplayImage()
        {
            if (_imageData == null) return;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.SetSource(new MemoryStream(_imageData).AsRandomAccessStream());
                ImagePreview.Source = bitmap;
                ImagePreview.Visibility = Visibility.Visible;
            }
            catch
            {
                ShowError("Could not load image.");
            }
        }

        private void SaveProductButton_Click(object sender, RoutedEventArgs e)
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
            var productToUpdate = db.Product.Find(_product.Id);

            if (productToUpdate == null)
            {
                ShowError("Product not found.");
                return;
            }

            if (productToUpdate.UserId != SessionManager.CurrentUser.Id)
            {
                ShowError("You can only edit your own products.");
                return;
            }

            productToUpdate.Name = name;
            productToUpdate.Description = description;
            productToUpdate.Material = material;
            productToUpdate.ProductionTime = productionTime;
            productToUpdate.Complexity = complexity;
            productToUpdate.Durability = durability;
            productToUpdate.UniqueFeatures = uniqueFeatures;
            productToUpdate.CatagoryId = categoryId;
            productToUpdate.Price = price;  

            if (_imageData != null)
                productToUpdate.ImageData = _imageData;

            db.Product.Update(productToUpdate);
            db.SaveChanges();
            Frame.Navigate(typeof(HomePage));   
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HomePage));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Delete Product",
                Content = "Are you sure you want to delete this product? This action cannot be undone.",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = XamlRoot
            };

            dialog.PrimaryButtonClick += (s, args) =>
            {
                using var db = new AppDataContext();
                var productToDelete = db.Product.Find(_product.Id);

                if (productToDelete != null && productToDelete.UserId == SessionManager.CurrentUser.Id)
                {
                    db.Product.Remove(productToDelete);
                    db.SaveChanges();
                    Frame.Navigate(typeof(HomePage));
                }
                else
                {
                    ShowError("Could not delete product.");
                }
            };

            _ = dialog.ShowAsync();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}