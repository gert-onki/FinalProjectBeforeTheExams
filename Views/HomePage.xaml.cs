using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using shittyEtsy.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace shittyEtsy.Views
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            using var db = new Data.AppDataContext();
            var products = db.Product.ToList();

            var viewModels = new List<ProductViewModel>();

            foreach (var p in products)
            {
                var vm = new ProductViewModel { Product = p };

                if (p.ImageData != null && p.ImageData.Length > 0)
                {
                    var bitmap = new BitmapImage();
                    using var ms = new MemoryStream(p.ImageData);
                    var stream = ms.AsRandomAccessStream();
                    await bitmap.SetSourceAsync(stream);
                    vm.Image = bitmap;
                }

                viewModels.Add(vm);
            }

            productsList.ItemsSource = viewModels;
        }

        private void GoToCreateProductPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateProductPage));
        }
    }
}