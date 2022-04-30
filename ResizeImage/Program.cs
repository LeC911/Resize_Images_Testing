namespace ConsoleApp;

public class Program
{
    /// <summary>
    ///  The main entry point for the program.
    /// </summary>
    static void Main(string[] args)
    {
        var pathToInputDirectory = @"C:\Users\LupusLunar\RiderProjects\ConsoleApp\TestProject\InputData";
        var pathToOutputDirectory = @"C:\Users\LupusLunar\RiderProjects\ConsoleApp\TestProject\OutputData";
        var images = Process.LoadImages(pathToInputDirectory);
        var resizedImages = Process.SaveResizedImages(images, pathToOutputDirectory);
    }
}