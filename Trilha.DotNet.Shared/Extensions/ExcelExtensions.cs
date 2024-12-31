namespace Trilha.DotNet.Shared.Extensions;

public static class ExcelExtensions
{
    public const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public static byte[] SerializeExcel<T>(this string worksheet, Dictionary<string, string> columns, IList<T> rows) where T : notnull
    {
        var dt = new DataTable(worksheet);
        foreach (var col in columns)
        {
            dt.Columns.Add(col.Value);
        }

        foreach (var row in rows)
        {
            var values = new object[columns.Count];
            var json = (JObject)JToken.FromObject(row);

            for (var i = 0; i < columns.Count; i++)
            {
                var colKey = columns.ElementAt(i).Key;
                values[i] = (!string.IsNullOrWhiteSpace(json[colKey]?.ToString()) ? json[colKey]?.ToString().Trim() : string.Empty) ?? string.Empty;
            }

            dt.Rows.Add(values);
        }

        using var wb = new XLWorkbook();
        wb.Worksheets.Add(dt);

        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        return stream.ToArray();
    }

    public static IList<DataRow> DeserializeExcel(this IFormFile file)
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
                var newRow = dt.NewRow();
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    newRow[i] = row.Cell(i + 1).Value.ToString() ?? string.Empty;
                }
                dt.Rows.Add(newRow);
            }
        }

        var convertedList = (from rw in dt.AsEnumerable() select rw).ToList();

        for (var i = 0; i < convertedList.Count; i++)
        {
            if (Array.TrueForAll(convertedList[i].ItemArray, field => string.IsNullOrWhiteSpace(field?.ToString())))
            {
                convertedList.RemoveAt(i);
            }
        }

        return convertedList;
    }
    
    public static byte[] AddExcelTab<T>(this string worksheet, byte[] excel, Dictionary<string, string> columns, IList<T> rows)
    {
        using var file = new MemoryStream(excel);
        using var workBook = new XLWorkbook(file);

        var dt = new DataTable(worksheet);
        dt.Columns.AddRange(columns.Values.Select(col => new DataColumn(col)).ToArray());

        foreach (var row in rows)
        {
            var values = columns.Keys.Select(col =>
            {
                var json = (JObject)JToken.FromObject(row!);
                return !string.IsNullOrWhiteSpace(json.GetValue(col)?.ToString()) ? json.GetValue(col)?.ToString().Trim() : string.Empty;
            })!.ToArray<object>();

            dt.Rows.Add(values);
        }

        workBook.Worksheets.Add(dt);

        using var stream = new MemoryStream();
        workBook.SaveAs(stream);

        return stream.ToArray();
    }
}
