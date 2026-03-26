using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using shittyEtsy.Data.Models;
using shittyEtsy.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace shittyEtsy.Views
{
    public sealed partial class HomePage : Page
    {
        private List<ProductViewModel> _allProducts = new();

        public HomePage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            using var db = new Data.AppDataContext();
            var products = db.Product.ToList();
            var categories = db.Categories.ToList();

            CategoryFilter.Items.Clear();
            CategoryFilter.Items.Add(new ComboBoxItem { Content = "All Categories", Tag = 0 });
            foreach (var cat in categories)
                CategoryFilter.Items.Add(new ComboBoxItem { Content = cat.Name, Tag = cat.Id });

            _allProducts = new List<ProductViewModel>();
            foreach (var p in products)
            {
                var vm = new ProductViewModel { Product = p };
                if (p.ImageData != null && p.ImageData.Length > 0)
                {
                    var bitmap = new BitmapImage();
                    using var ms = new MemoryStream(p.ImageData);
                    await bitmap.SetSourceAsync(ms.AsRandomAccessStream());
                    vm.Image = bitmap;
                }
                _allProducts.Add(vm);
            }

            var materials = _allProducts
                .Select(vm => vm.Material)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Distinct()
                .OrderBy(m => m)
                .ToList();

            MaterialFilter.Items.Clear();
            MaterialFilter.Items.Add(new ComboBoxItem { Content = "All Materials", Tag = "" });
            foreach (var mat in materials)
                MaterialFilter.Items.Add(new ComboBoxItem { Content = mat, Tag = mat });

            ApplyFilters();
        }

        private void Filter_Changed(object sender, object e)
        {
            ApplyFilters();
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            CategoryFilter.SelectedIndex = -1;
            ComplexityFilter.SelectedIndex = -1;
            MaterialFilter.SelectedIndex = -1;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allProducts.AsEnumerable();

            var search = SearchBox?.Text?.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(vm => vm.Name?.ToLower().Contains(search) == true);

            if (CategoryFilter?.SelectedItem is ComboBoxItem { Tag: int catId } && catId != 0)
                filtered = filtered.Where(vm => vm.Product.CatagoryId == catId);

            if (ComplexityFilter?.SelectedItem is ComboBoxItem complexityItem
                && complexityItem.Content?.ToString() is string complexity
                && complexity != "All Complexities")
                filtered = filtered.Where(vm => vm.Complexity == complexity);

            if (MaterialFilter?.SelectedItem is ComboBoxItem { Tag: string mat }
                && !string.IsNullOrEmpty(mat))
                filtered = filtered.Where(vm => vm.Material == mat);

            productsList.ItemsSource = filtered.ToList();
        }

        private void GoToCreateProductPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateProductPage));
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int productId)
                Frame.Navigate(typeof(EditProductPage), productId);
        }
    }
}