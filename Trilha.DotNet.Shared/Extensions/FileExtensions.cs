namespace Trilha.DotNet.Shared.Extensions;

public static class FileExtensions
{
    public static string GetContentType(this string filename)
    {
        new FileExtensionContentTypeProvider().
            TryGetContentType(filename, out var contentType);

        return contentType ?? MediaTypeNames.Application.Octet;
    }

    public static string Compress<T>(this T data) where T : notnull
    {
        var uncompressedString = data.Stringify();
        var uncompressedBytes = Encoding.UTF8.GetBytes(uncompressedString);

        using var input = new MemoryStream(uncompressedBytes);
        using var output = new MemoryStream();
        using (var brotli = new BrotliStream(output, CompressionLevel.Optimal))
        {
            input.CopyTo(brotli);
        }

        return Convert.ToBase64String(output.ToArray());
    }

    public static T Decompress<T>(this string compressedString) where T : notnull
    {
        var compressedBase64 = Convert.FromBase64String(compressedString);

        using var input = new MemoryStream(compressedBase64);
        using var output = new MemoryStream();
        using (var brotli = new BrotliStream(input, CompressionMode.Decompress))
        {
            brotli.CopyTo(output);
        }

        var decompressedBytes = output.ToArray();
        var decompressedString = Encoding.UTF8.GetString(decompressedBytes);
        return decompressedString.ParseJson<T>();
    }
    
    public static MemoryStream ToMemoryStream(this byte[] file) => new(file);
}
