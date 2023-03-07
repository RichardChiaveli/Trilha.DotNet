namespace Trilha.DotNet.Shared.Extensions;

public static class WebScrapingExtensions
{
    private static readonly HttpClient HttpClient = new();

    private static async Task<HtmlDocument> Load(this string url)
    {
        HttpClient.BaseAddress = new Uri(url);
        var response = await HttpClient.Invoke(HttpMethod.Get);
        var bytes = await response.ResponseAsByteArrayAsync();

        var htmlContent = Encoding.UTF8.GetString(bytes);
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        return htmlDoc;
    }

    public static async Task<IEnumerable<HtmlNode>> GetElement(this string url, string selector)
        => (await Load(url)).DocumentNode.QuerySelectorAll(selector);
}