using System;
using System.Threading.Tasks;
using GlobalGeekTools;
using HtmlAgilityPack;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Starting single page download...");

        var downloader = new SinglePageDownloader();

        await downloader.DownloadSpecificPageAsync(

             "https://example.com/about/team",    // Replace with your target page
             @"C:\GeekOutput\TeamPage"            // Replace with your desired output folder
        );

        Console.WriteLine("Operation completed sir.");
    }
}
