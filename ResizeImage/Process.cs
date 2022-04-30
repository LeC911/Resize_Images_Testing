using System.Drawing;

namespace ConsoleApp;

public class Process
{
    /// <summary>
    /// The method loads images from a directory into the dictionary.
    /// </summary>
    /// <param name="pathToDir">The path to the directory with images.</param>
    /// <returns>A dictionary with file names (string) and images (bitmap object).</returns>
    public static Dictionary<string, Bitmap> LoadImages(string pathToDir)
    {
        var images = new Dictionary<string, Bitmap>();
        foreach (var file in Directory.GetFiles(pathToDir))
        {
            if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".bmp"))
            {
                try
                {
                    images.Add(Path.GetFileName(file), new Bitmap(file));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine($"Method: {ex.TargetSite}");
                }
            }
        }

        return images;
    }

    /// <summary>
    /// The method calls the method to resize images and saves them to a new folder.
    /// </summary>
    /// <param name="imagesDictionary">The dictionary with file names (string) and images (bitmap object).</param>
    /// <param name="pathToOutDir">The path to the directory to save the resized images.</param>
    /// <returns>A dictionary with file names (string) and resized images (bitmap object).</returns>
    public static Dictionary<string, Bitmap> SaveResizedImages(Dictionary<string, Bitmap> imagesDictionary, string pathToOutDir)
    {
        var newWidth = 200;
        var newHeight = 250;
        var resizedImages = new Dictionary<string, Bitmap>();


        foreach (var image in imagesDictionary)
        {
            var resizedImage = Resizer.Resize(image.Value, newWidth, newHeight);
            var ext = Path.GetExtension(image.Key);
            resizedImage.Save(Path.Combine(pathToOutDir,
                image.Key.Replace(ext, $"_{newWidth}x{newHeight}{ext}")));
            newWidth += 200;
            newHeight += 200;
        }
        
        foreach (var file in Directory.GetFiles(pathToOutDir))
        {
            if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".bmp"))
            {
                try
                {
                    resizedImages.Add(Path.GetFileName(file), new Bitmap(file));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine($"Method: {ex.TargetSite}");
                }
            }
        }

        return resizedImages;
    }
}