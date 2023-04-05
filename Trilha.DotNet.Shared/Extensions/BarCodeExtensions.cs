namespace Trilha.DotNet.Shared.Extensions;

public static class BarCodeExtensions
{
    public static string CreateBase64Image(this BarcodeFormat format, string content, int width, int height)
    {
        const int margin = 0;
        var qrCodeWriter = new BarcodeWriterPixelData
        {
            Format = format,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width,
                Margin = margin
            }
        };

        var pixelData = qrCodeWriter.Write(content);

        using var image = Image.LoadPixelData<Rgba32>(pixelData.Pixels, width, height);
        using var memoryStream = new MemoryStream();

        image.Save(memoryStream, new PngEncoder
        {
            SkipMetadata = true
        });
        var byteArray = memoryStream.ToArray();
        return byteArray.Length > 0 ? Convert.ToBase64String(byteArray) : string.Empty;
    }
}
