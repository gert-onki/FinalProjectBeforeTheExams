using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;

namespace shittyEtsy.Converters
{
    public class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not byte[] bytes || bytes.Length == 0)
                return null;

            var bitmap = new BitmapImage();
            using var ms = new MemoryStream(bytes);
            var stream = ms.AsRandomAccessStream();
            bitmap.SetSourceAsync(stream).GetResults();
            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}