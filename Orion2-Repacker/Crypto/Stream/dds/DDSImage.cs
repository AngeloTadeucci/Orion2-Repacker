using System.Drawing.Imaging;
using Pfim;
using ImageFormat = Pfim.ImageFormat;

namespace Orion.Crypto.Stream.DDS; 
public class DDSImage : IDisposable {
    private readonly IImage _image;

    public DDSImage(byte[] ddsImage) {
        if (ddsImage == null)
            return;

        if (ddsImage.Length == 0)
            return;

        _image = Dds.Create(ddsImage, new PfimConfig());
        Parse();
    }

    public DDSImage(System.IO.Stream ddsImage) {
        if (ddsImage == null)
            return;

        if (!ddsImage.CanRead)
            return;

        _image = Dds.Create(ddsImage, new PfimConfig());
        Parse();
    }

    public Bitmap BitmapImage { get; private set; }

    public void Dispose() {
        if (BitmapImage != null) {
            BitmapImage.Dispose();
            BitmapImage = null;
        }
    }

    private void Parse() {
        if (_image == null)
            throw new Exception("Image data failed to create within Pfim");

        if (_image.Compressed)
            _image.Decompress();

        BitmapImage = CreateBitmap(_image);
    }

    private Bitmap CreateBitmap(IImage image) {
        PixelFormat pxFormat = PixelFormat.Format24bppRgb;
        if (image.Format == ImageFormat.Rgba32)
            pxFormat = PixelFormat.Format32bppArgb;

        unsafe {
            fixed (byte* bytePtr = image.Data) {
                return new Bitmap(image.Width, image.Height, image.Stride, pxFormat, (IntPtr)bytePtr);
            }
        }
    }
}