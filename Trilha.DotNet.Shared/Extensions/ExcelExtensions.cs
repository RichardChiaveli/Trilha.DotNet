namespace Trilha.DotNet.Shared.Extensions;

public static class ExcelExtensions
{
    public const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public static byte[] SerializeExcel<T>(this string worksheet, Dictionary<string, string> columns, IList<T> rows) where T : notnull
    {
        var dt = new DataTable(worksheet);
        dt.Columns.AddRange(columns.Select(col => new DataColumn(col.Value)).ToArray());

        foreach (var row in rows)
        {
            var values = (from col in columns
                          let json = (JObject)JToken.FromObject(row)
                          select !string.IsNullOrWhiteSpace(json[col.Key]?.ToString()) ? json[col.Key]?.ToString()?.Trim() : string.Empty).ToArray<object>();

            dt.Rows.Add(values);
        }

        using var wb = new XLWorkbook();
        wb.Worksheets.Add(dt);

        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        return stream.ToArray();
    }

    public static List<DataRow> DeserializeExcel(this IFormFile file)
    {
        using var workBook = new XLWorkbook(file.OpenReadStream());
        var workSheet = workBook.Worksheet(1);

        var dt = new DataTable();
        var firstRow = true;

        foreach (var row in workSheet.Rows())
        {
            if (firstRow)
            {
                foreach (var cell in row.CellsUsed())
                {
                    dt.Columns.Add(cell.Value.ToString());
                }
                firstRow = false;
            }
            else
            {
                dt.Rows.Add();
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    dt.Rows[^1][i] = row.Cell(i + 1).Value.ToString() ?? string.Empty;
                }
            }
        }

        var convertedList = (from rw in dt.AsEnumerable() select rw).ToList();

        for (var i = 0; i < convertedList.Count; i++)
        {
            if (convertedList[i].ItemArray.All(field => string.IsNullOrWhiteSpace(field?.ToString())))
            {
                convertedList.RemoveAt(i);
            }
        }

        return convertedList;
    }

    public static byte[] AddExcelTab<T>(this string worksheet, byte[] excel, Dictionary<string, string> columns, IList<T> rows)
    {
        var file = new MemoryStream(excel);
        using var workBook = new XLWorkbook(file);

        var dt = new DataTable(worksheet);
        dt.Columns.AddRange(columns.Select(col => new DataColumn(col.Value)).ToArray());

        foreach (var row in rows)
        {
            var values = (from col in columns
                          let json = (JObject)JToken.FromObject(row!)
                          select !string.IsNullOrWhiteSpace(json[col.Key]?.ToString()) ? json[col.Key!]!.ToString().Trim() : string.Empty).ToArray<object>();

            dt.Rows.Add(values);
        }

        workBook.Worksheets.Add(dt);

        using var stream = new MemoryStream();

        workBook.SaveAs(stream);

        return stream.ToArray();
    }
}
