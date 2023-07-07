namespace Trilha.DotNet.Shared.Extensions;

public static class PdfExtensions
{
    public static byte[] SerializePdf(
        this IConverter converter
        , StringBuilder html
        , GlobalSettings? settings = null
        , HeaderSettings? header = null
        , FooterSettings? footer = null)
    {
        var htmlToPdfDocument = new HtmlToPdfDocument
        {
            GlobalSettings = settings ?? new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 25, Bottom = 25 }
            },
            Objects =
                {
                    new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = html.ToString(),
                        WebSettings = new WebSettings
                        {
                            DefaultEncoding = "UTF-8",
                        },
                        HeaderSettings = header ?? new HeaderSettings
                        {
                            FontSize = 15,
                            FontName = "Ariel",
                            Right = "Page [page] of [toPage]",
                            Line = true
                        },
                        FooterSettings = footer ?? new FooterSettings
                        {
                            FontSize = 12,
                            FontName = "Ariel",
                            Line = true
                        }
                    }
                }
        };

        return converter.Convert(htmlToPdfDocument);
    }
}
