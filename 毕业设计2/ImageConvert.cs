using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace 毕业设计2
{
    public class ImageConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] musicImage = (byte[])value;
            BitmapImage image = null;
            if (musicImage != null)
            {
                MemoryStream ms = new MemoryStream(musicImage);
                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();
            }
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
