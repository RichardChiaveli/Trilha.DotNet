namespace Trilha.DotNet.Shared.Extensions;

public static class CsvExtensions
{
    public static IEnumerable<T> DeserializeCsv<T>(this IFormFile file) where T : class
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, config);

        var result = csv.GetRecords<T>();

        return result;
    }

    public static byte[] SerializeCsv<T>(this IEnumerable<T> records) where T : class
    {
        if (records == null) throw new ArgumentNullException(nameof(records));

        using var memoryStream = new MemoryStream();
        using (var streamWriter = new StreamWriter(memoryStream, leaveOpen: true))
        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(records);
            streamWriter.Flush();
        }

        return memoryStream.ToArray();
    }
}