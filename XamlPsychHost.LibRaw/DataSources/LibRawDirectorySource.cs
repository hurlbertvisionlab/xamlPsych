using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class LibRawDirectorySource : ImagesDirectorySource
    {
        public override ImageItem GetImageItem(string file)
        {
            LibRawWrapper.LibRawBitmapDecoder raw = new LibRawWrapper.LibRawBitmapDecoder(new Uri(file), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            BitmapSource bitmap = raw.Frames[0];

            return new ImageItem
            {
                Source = file,
                Name = Path.GetFileName(file),
                Image = bitmap
            };
        }
    }
}
