using AngleSharp;
using AngleSharp.Dom;
using PuppeteerSharp;

using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
var browser = await Puppeteer.LaunchAsync(new LaunchOptions
{
    Headless = true
});
var page = await browser.NewPageAsync();
await page.GoToAsync("https://www.scrapethissite.com/pages/simple/");

var content = await page.GetContentAsync();

var context = BrowsingContext.New(Configuration.Default);
var document = await context.OpenAsync(req => req.Content(content));

var countries = document.QuerySelectorAll("*")
    .Where(e => e.LocalName == "div" && e.ClassName == "col-md-4 country")
    .ToList();

List<Country> parsedCountries = new List<Country>();
foreach (var c in countries)
{
    var lines = c.Text().Split("\n").Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

    parsedCountries.Add(new Country(
        lines[0].Trim(),
        lines[1].Split(':')[1],
        int.Parse(lines[2].Split(':')[1]),
        (int)float.Parse(lines[3].Split(':')[1]))
    );
}

foreach (var country in parsedCountries)
    Console.WriteLine(country.Name);

Console.ReadLine();

public record Country(string Name, string Capital, int Population, int Area);