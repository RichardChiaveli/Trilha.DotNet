namespace Trilha.DotNet.Shared.Extensions;

public static class WebScrapingExtensions
{
    private static async Task<HtmlDocument> GetHtmlPageAsync(this HttpClient http, string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));
        }

        var response = await http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Failed to fetch the URL: {url}. Success code: {response.StatusCode}");
        }

        var htmlContent = await response.Content.ReadAsStringAsync();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        return htmlDoc;
    }

    public static async Task<IEnumerable<HtmlNode>> GetHtmlElementsAsync(this HttpClient http, string url, string selector)
    {
        if (string.IsNullOrWhiteSpace(selector))
        {
            throw new ArgumentException("Selector cannot be null or empty.", nameof(selector));
        }

        var htmlPage = await GetHtmlPageAsync(http, url);
        return htmlPage.DocumentNode.QuerySelectorAll(selector);
    }

}