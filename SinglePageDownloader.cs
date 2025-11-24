using System;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace GlobalGeekTools
{
    public class SinglePageDownloader
    {
        private readonly HttpClient _client;

        public SinglePageDownloader()
        {
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            _client.DefaultRequestHeaders.Add("User-Agent", "GlobalGeekDownloader");
        }

        public async Task DownloadSpecificPageAsync(string pageUrl, string outputFolder)
        {
            Directory.CreateDirectory(outputFolder);

            Console.WriteLine("Fetching target page...");
            string html = await _client.GetStringAsync(pageUrl);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            Uri baseUri = new Uri(pageUrl);

            var assets = new List<(string url, string attribute, HtmlNode node)>();

            CollectStandardAssets(htmlDoc, baseUri, assets);

            await DownloadAssetsParallel(assets, outputFolder);

            await ProcessCssFilesForBackgroundImages(outputFolder, baseUri);

            string outputName = GetSafeFileNameFromUrl(pageUrl);
            string outputFile = Path.Combine(outputFolder, outputName);

            htmlDoc.Save(outputFile);

            Console.WriteLine("Page and assets saved successfully.");
            Console.WriteLine("Saved at: " + outputFile);
        }

        private void CollectStandardAssets(HtmlDocument doc, Uri baseUri, List<(string, string, HtmlNode)> assets)
        {
            var rules = new List<string>
            {
                "//img[@src]",
                "//script[@src]",
                "//link[@href]"
            };

            foreach (var rule in rules)
            {
                var nodes = doc.DocumentNode.SelectNodes(rule);
                if (nodes == null) continue;

                foreach (var node in nodes)
                {
                    string attribute = rule.Contains("@src") ? "src" : "href";
                    string value = node.GetAttributeValue(attribute, null);

                    if (string.IsNullOrWhiteSpace(value)) continue;

                    Uri absolute = new Uri(baseUri, value);

                    assets.Add((absolute.ToString(), attribute, node));
                }
            }
        }

        private async Task DownloadAssetsParallel(List<(string url, string attribute, HtmlNode node)> assets, string folder)
        {
            var tasks = new List<Task>();

            foreach (var asset in assets)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        Uri uri = new Uri(asset.url);
                        string fileName = Path.GetFileName(uri.LocalPath);

                        if (string.IsNullOrWhiteSpace(fileName))
                        {
                            fileName = Guid.NewGuid().ToString("N");
                        }

                        string localPath = Path.Combine(folder, fileName);

                        byte[] data = await _client.GetByteArrayAsync(uri);
                        await File.WriteAllBytesAsync(localPath, data);

                        asset.node.SetAttributeValue(asset.attribute, fileName);
                    }
                    catch
                    {
                        Console.WriteLine("Failed asset: " + asset.url);
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

        private async Task ProcessCssFilesForBackgroundImages(string folder, Uri baseUri)
        {
            string[] cssFiles = Directory.GetFiles(folder, "*.css");

            foreach (var cssPath in cssFiles)
            {
                string css = await File.ReadAllTextAsync(cssPath);

                var matches = Regex.Matches(css, @"url\(['""]?(.*?)['""]?\)");

                foreach (Match match in matches)
                {
                    string raw = match.Groups[1].Value;

                    if (raw.StartsWith("data", StringComparison.OrdinalIgnoreCase))
                        continue;

                    Uri absolute = new Uri(baseUri, raw);

                    try
                    {
                        string fileName = Path.GetFileName(absolute.LocalPath);

                        if (string.IsNullOrWhiteSpace(fileName))
                        {
                            fileName = Guid.NewGuid().ToString("N");
                        }

                        string assetPath = Path.Combine(folder, fileName);

                        byte[] data = await _client.GetByteArrayAsync(absolute);
                        await File.WriteAllBytesAsync(assetPath, data);

                        css = css.Replace(raw, fileName);
                    }
                    catch
                    {
                        Console.WriteLine("Failed CSS image: " + absolute);
                    }
                }

                await File.WriteAllTextAsync(cssPath, css);
            }
        }

        private string GetSafeFileNameFromUrl(string url)
        {
            Uri uri = new Uri(url);
            string segment = uri.Segments[^1];

            if (string.IsNullOrWhiteSpace(segment))
                segment = "page.html";

            if (!segment.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                segment += ".html";

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                segment = segment.Replace(c.ToString(), "_");
            }

            return segment;
        }
    }
}
