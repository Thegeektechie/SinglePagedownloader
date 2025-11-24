 Single Page Downloader

This project provides a precise and efficient tool that downloads only a specified webpage along with all assets that the page directly references. The system does not download the full site. It focuses on the selected page alone and rewrites asset paths so the page can render correctly offline.

The downloader supports parallel asset retrieval, extraction of background images from CSS files, local path rewriting, and clean output structure. It is built with C sharp using modern .NET standards.



 Features

 Targeted Page Retrieval

Downloads the exact page you specify rather than the site root.

 Parallel Asset Downloading

All referenced assets are fetched concurrently for high performance.

 Comprehensive Asset Detection

Images, scripts, stylesheets, and resources referenced inside CSS files are captured.

 CSS Background Image Extraction

Parses CSS content and retrieves any image referenced through background image declarations.

 Local Path Rewriting

Rewrites all asset references inside HTML and CSS files to ensure full offline functionality.

 Clean Output Structure

Stores everything in a specified folder with no unnecessary files.



 Requirements

To build and run this project you need:

 .NET 6 or newer
 HtmlAgilityPack NuGet package
 A stable internet connection for downloading the target page and assets

Install HtmlAgilityPack using:

```
dotnet add package HtmlAgilityPack
```



 Project Setup

Follow these steps to configure the project.

 One

Clone or copy the project files into your preferred directory.

 Two

Navigate into the project folder:

```
cd PageDownloader
```

 Three

Install the required package:

```
dotnet add package HtmlAgilityPack
dotnet restore
```

 Four

Build the project:

```
dotnet build
```

 Five

Run the program:

```
dotnet run
```



 Usage

Open Program.cs and modify the following line to set your desired page and output folder.

```csharp
await downloader.DownloadSpecificPageAsync(
    "https://example.com/about/team",
    @"C:\GeekOutput\TeamPage"
);
```

Then run the application. The system will:

One. Fetch the specified page
Two. Extract every referenced asset
Three. Download all items in parallel
Four. Process CSS content to extract background images
Five. Rewrite asset paths so the page loads completely offline
Six. Save the final page and all assets into your output folder



 Output Structure

Your output folder will contain:

 The rewritten HTML file
 All images
 All script files
 All CSS files
 All background images extracted from CSS

The final HTML file is named based on the last segment of the page URL.



 Error Handling

The system reports any assets it cannot fetch. In such cases the download process continues without interruption.
If you require retry logic or extended diagnostics, these can be added easily.



 Extending the Project

This project can be extended with any of the following capabilities:

 Command line parameters for dynamic URL input
 Automatic packaging of the downloaded page into a zip file
 A graphical interface using WinUI or WPF
 Logging to JSON or plain text
 Full asset dependency resolution
 Rate limiting controls
 Browser user agent switching

If you require these enhancements, they can be integrated seamlessly.



 Credits

Developed by Geek Techie
This project demonstrates precise engineering, modern C sharp design, and clear architecture for web asset retrieval.