using Microsoft.UI.Xaml.Media.Imaging;
using shittyEtsy.Data.Models;
using shittyEtsy.Session;

namespace shittyEtsy.Views
{
    public class ProductViewModel
    {
        public Products Product { get; set; }
        public BitmapImage Image { get; set; }
        public string Name => Product.Name;
        public string Description => Product.Description;
        public string Material => Product.Material;
        public string Complexity => Product.Complexity;
        public string Durability => Product.Durability;
        public string ProductionTime => Product.ProductionTime;
        public string UniqueFeatures => Product.UniqueFeatures;
        public int ProductId => Product.Id;
        public int UserId => Product.UserId;
        public bool IsOwnProduct => SessionManager.CurrentUser != null && Product.UserId == SessionManager.CurrentUser.Id;
    }
}
