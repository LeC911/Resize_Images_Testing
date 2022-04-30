using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ConsoleApp;

public class Resizer
{
    /// <summary>
    /// The method resizes image.
    /// </summary>
    /// <param name="image">The original image.</param>
    /// <param name="newW">The desired width.</param>
    /// <param name="newH">The desired height.</param>
    /// <returns>A bitmap object, resized image.</returns>
    public static Bitmap Resize(Bitmap image, int newW, int newH)
    {
        var (origWidth, origHeight) = (image.Width, image.Height);
        if (origWidth != newW || origHeight != newH)
        {
            var bitImage = new Bitmap(newW, newH);
            var isOldWidthTheLargest = origWidth / origHeight > newW / newH;

            if (isOldWidthTheLargest)
                newH = Math.Min(origHeight * newW / origWidth, newH);
            else
                newW = Math.Min(origWidth * newH / origHeight, newW);

            if (newW < origWidth || newH < origHeight)
            {
                // Применим Гауссов фильтр для предотвращения алиасинга при уменьшении изображения.
                var kernel = GaussianBlur(3, 1.5);
                image = Convolve(image, kernel);
            }
            
            bitImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(bitImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, newW, newH);
            }

            image = bitImage;
        }

        return image;
    }

    private static double[,] GaussianBlur(int lenght, double weight)
    {
        double[,] kernel = new double[lenght, lenght];
        double kernelSum = 0;
        int foff = (lenght - 1) / 2;
        double distance = 0;
        double constant = 1d / (2 * Math.PI * weight * weight);
        for (int y = -foff; y <= foff; y++)
        {
            for (int x = -foff; x <= foff; x++)
            {
                distance = ((y * y) + (x * x)) / (2 * weight * weight);
                kernel[y + foff, x + foff] = constant * Math.Exp(-distance);
                kernelSum += kernel[y + foff, x + foff];
            }
        }
        for (int y = 0; y < lenght; y++)
        {
            for (int x = 0; x < lenght; x++)
            {
                kernel[y, x] = kernel[y, x] * 1d / kernelSum;
            }
        }
        return kernel;
    }
    
    private static Bitmap Convolve(Bitmap srcImage, double[,] kernel)
    {
        int width = srcImage.Width;
        int height = srcImage.Height;
        BitmapData srcData = srcImage.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        int bytes = srcData.Stride * srcData.Height;
        byte[] buffer = new byte[bytes];
        byte[] result = new byte[bytes];
        Marshal.Copy(srcData.Scan0, buffer, 0, bytes);
        srcImage.UnlockBits(srcData);
        int colorChannels = 3;
        double[] rgb = new double[colorChannels];
        int foff = (kernel.GetLength(0) - 1) / 2;
        int kcenter = 0;
        int kpixel = 0;
        for (int y = foff; y < height - foff; y++)
        {
            for (int x = foff; x < width - foff; x++)
            {
                for (int c = 0; c < colorChannels; c++)
                {
                    rgb[c] = 0.0;
                }
                kcenter = y * srcData.Stride + x * 4;
                for (int fy = -foff; fy <= foff; fy++)
                {
                    for (int fx = -foff; fx <= foff; fx++)
                    {
                        kpixel = kcenter + fy * srcData.Stride + fx * 4;
                        for (int c = 0; c < colorChannels; c++)
                        {
                            rgb[c] += (double)(buffer[kpixel + c]) * kernel[fy + foff, fx + foff];
                        }
                    }
                }
                for (int c = 0; c < colorChannels; c++)
                {
                    if (rgb[c] > 255)
                    {
                        rgb[c] = 255;
                    }
                    else if (rgb[c] < 0)
                    {
                        rgb[c] = 0;
                    }
                }
                for (int c = 0; c < colorChannels; c++)
                {
                    result[kcenter + c] = (byte)rgb[c];
                }
                result[kcenter + 3] = 255;
            }
        }
        
        Bitmap resultImage = new Bitmap(width, height);
        BitmapData resultData = resultImage.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        Marshal.Copy(result, 0, resultData.Scan0, bytes);
        resultImage.UnlockBits(resultData);
        
        return resultImage;
    }
}