using System.Drawing;
using NUnit.Framework;
using ConsoleApp;

namespace TestProject;

public class Tests
{

    [TestCase(@"C:\Users\LupusLunar\RiderProjects\ConsoleApp\TestProject\InputData", 3)]
    public void LoadImagesFromInputDirectory(string inputDir, int imgCount)
    {
        var images = Process.LoadImages(inputDir);
        Assert.NotNull(images);
        Assert.AreEqual(imgCount, images.Count);
    }
    
    [TestCase(@"C:\Users\LupusLunar\RiderProjects\ConsoleApp\TestProject\InputData", 
        @"C:\Users\LupusLunar\RiderProjects\ConsoleApp\TestProject\OutputData", 3)]
    public void SaveImagesToOutputDirectory(string inputDir, string outputDir, int imgCount)
    {
        var images = Process.LoadImages(inputDir);
        var resizedImages = Process.SaveResizedImages(images, outputDir);
        Assert.NotNull(resizedImages);
        Assert.AreEqual(imgCount, resizedImages.Count);
        Assert.AreEqual(images.Count, resizedImages.Count);
    }

    [TestCase(@"C:\Users\LupusLunar\RiderProjects\ConsoleApp\TestProject\InputData\LOTR.jpg",
        500, 700)]
    public void CheckChangedImageSize(string imageName, int newW, int newH)
    {
        var image = new Bitmap(imageName);
        var resizedImage = Resizer.Resize(image, newW, newH);
        Assert.NotNull(resizedImage);
        Assert.AreEqual(newW, resizedImage.Width);
        Assert.AreEqual(newH, resizedImage.Height);
    }
    
    [TestCase(@"C:\Users\LupusLunar\RiderProjects\ConsoleApp\TestProject\InputData\cat.jpg",
        800, 480)]
    public void CheckSameImageSize(string imageName, int newW, int newH)
    {
        var image = new Bitmap(imageName);
        var (origWidth, origHeight) = (image.Width, image.Height);
        var resizedImage = Resizer.Resize(image, newW, newH);
        Assert.NotNull(resizedImage);
        Assert.AreEqual(origWidth, resizedImage.Width);
        Assert.AreEqual(origHeight, resizedImage.Height);
    }
}