namespace Trilha.DotNet.Shared.Extensions;

public static class WebServiceExtensions
{
    private static HttpRequestMessage Initialize(
        this Uri url
        , HttpMethod httpMethod
        , object? payload = null
        , params KeyValuePair<string, string>[] headers)
    {
        var request = new HttpRequestMessage
        {
            Method = httpMethod,
            RequestUri = url
        };

        request.Headers.Clear();

        if (httpMethod != HttpMethod.Get && payload != null)
        {
            request.Content = payload switch
            {
                MultipartFormDataContent content => content,
                StringContent stringContent => stringContent,
                FormUrlEncodedContent formUrlEncodedContent => formUrlEncodedContent,
                _ => payload.FromJsonBody()
            };
        }

        if (!headers.Any()) return request;

        var contentList = new List<Type>
            {
                typeof(MultipartFormDataContent),
                typeof(StringContent),
                typeof(FormUrlEncodedContent)
            };

        if (payload is null || (request.Content != null && contentList.Contains(request.Content.GetType())))
        {
            headers = headers.Where(i => i.Key != "Content-Type").ToArray();
        }

        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        return request;
    }

    public static async Task<HttpResponseMessage> Invoke(
        this HttpClient httpClient
        , HttpMethod httpMethod
        , string? relativeUri = null
        , object? payload = default!
        , bool throwExceptionNoInternet = false
        , params KeyValuePair<string, string>[] headers)
    {
        try
        {
            var request = new Uri(httpClient.BaseAddress!, relativeUri).Initialize(httpMethod, payload, headers);
            return await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            if (e is HttpRequestException or TaskCanceledException && !throwExceptionNoInternet)
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);

            throw;
        }
    }

    public static async Task<HttpResponseMessage> Invoke(
        this HttpClient httpClient
        , HttpMethod httpMethod
        , IFormFileCollection files
        , string? relativeUri = null
        , MultipartFormDataContent? fromForm = null
        , bool throwExceptionNoInternet = false
        , params KeyValuePair<string, string>[] headers)
    {
        try
        {
            fromForm ??= new MultipartFormDataContent();

            foreach (var file in files)
            {
                var content = await file.FromFormDataFile();
                fromForm.Add(content);
            }

            var request = new Uri(httpClient.BaseAddress!, relativeUri).Initialize(httpMethod, fromForm, headers);
            return await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            if (e is HttpRequestException or TaskCanceledException && !throwExceptionNoInternet)
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);

            throw;
        }
    }

    public static async Task<HttpResponseMessage> InvokeSoap(
        this HttpClient httpClient
        , HttpMethod httpMethod
        , string? relativeUri = null
        , string? action = null
        , string? xml = null
        , bool throwExceptionNoInternet = false)
    {
        try
        {
            var payload = string.IsNullOrWhiteSpace(xml) ? null :
                new StringContent(xml, Encoding.UTF8, "text/xml");

            var headers = string.IsNullOrWhiteSpace(action) ? default :
                new KeyValuePair<string, string>("SOAPAction", action);

            var request = new Uri(httpClient.BaseAddress!, relativeUri).Initialize(
                httpMethod, payload, headers);

            return await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        }
        catch (Exception e)
        {
            if (e is HttpRequestException or TaskCanceledException && !throwExceptionNoInternet)
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);

            throw;
        }
    }

    public static async Task<string> ResponseAsStringAsync(this HttpResponseMessage response)
    {
        var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        response.Dispose();

        return result;
    }

    public static string ResponseAsString(this HttpResponseMessage response)
        => ResponseAsStringAsync(response).ConfigureAwait(false).GetAwaiter().GetResult();

    public static async Task<T?> ResponseAsync<T>(this HttpResponseMessage response, bool isJson = true)
    {
        var data = await response.ResponseAsStringAsync();
        return isJson ? data.ParseJson<T>() : (T)Convert.ChangeType(data, typeof(T));
    }

    public static T? Response<T>(this HttpResponseMessage response, bool isJson = true)
        => ResponseAsync<T>(response, isJson).ConfigureAwait(false).GetAwaiter().GetResult();

    public static async Task<JObject> ResponseAsJObjectAsync(this HttpResponseMessage response)
    {
        var data = await response.ResponseAsStringAsync();
        return JObject.Parse(data);
    }

    public static JObject ResponseAsJObject(this HttpResponseMessage response)
        => ResponseAsJObjectAsync(response).ConfigureAwait(false).GetAwaiter().GetResult();

    public static async Task<byte[]> ResponseAsByteArrayAsync(this HttpResponseMessage response)
    {
        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        using var ms = new MemoryStream();
        await responseStream.CopyToAsync(ms);

        response.Dispose();

        return ms.ToArray();
    }

    public static byte[] ResponseAsByteArray(this HttpResponseMessage response)
        => ResponseAsByteArrayAsync(response).ConfigureAwait(false).GetAwaiter().GetResult();

    public static async Task<(string FileName, string ContentType, byte[] File)> ResponseAsFileAsync(this HttpResponseMessage response)
    {
        var content = response.Content;
        var header = content.Headers;

        var fileName = header.ContentDisposition?.FileName ?? string.Empty;
        var contentType = fileName.GetContentType();

        if (!response.IsSuccessStatusCode)
            throw new StatusCodeException(HttpStatusCode.BadRequest, "File Not Found.");

        var file = await content.ReadAsByteArrayAsync().ConfigureAwait(false);

        return new ValueTuple<string, string, byte[]>(fileName, contentType, file);
    }

    public static (string FileName, string ContentType, byte[] File) ResponseAsFile(this HttpResponseMessage response)
        => ResponseAsFileAsync(response).ConfigureAwait(false).GetAwaiter().GetResult();

    public static async Task<T> ResponseAsSoapAsync<T>(this HttpResponseMessage response)
    {
        var soap = await response.ResponseAsStringAsync();
        var serializer = new XmlSerializer(typeof(T));
        var xml = new ObjectXml.XmlDocument();
        xml.LoadXml(soap);

        const string xmlSchemaEnvelope = "http://schemas.xmlsoap.org/soap/envelope/";
        const string xmlSchemaDefinition = "http://www.w3.org/2001/XMLSchema";
        const string xmlSchemaInstance = "http://www.w3.org/2001/XMLSchema-instance";

        var manager = new ObjectXml.XmlNamespaceManager(xml.NameTable);
        manager.AddNamespace("s", xmlSchemaEnvelope);
        manager.AddNamespace("xsd", xmlSchemaDefinition);
        manager.AddNamespace("xsi", xmlSchemaInstance);

        var body = xml.SelectSingleNode("//s:Body", manager);
        var first = body?.FirstChild?.FirstChild;

        if (first == null)
            return default!;

        using ObjectXml.XmlReader reader = new ObjectXml.XmlNodeReader(first);
        var serialize = serializer.Deserialize(reader);

        if (serialize is T result)
            return result;

        return default!;
    }

    public static T ResponseAsSoap<T>(this HttpResponseMessage response)
        => ResponseAsSoapAsync<T>(response).ConfigureAwait(false).GetAwaiter().GetResult();

    public static StringContent FromJsonBody(
        this object? fromBody, string? mediaType = null) =>
        new(fromBody as string ?? fromBody!.Stringify(), Encoding.UTF8, mediaType ?? MediaTypeNames.Application.Json);

    public static MultipartFormDataContent FromFormData(this Dictionary<string, object> fromForm)
    {
        var content = new MultipartFormDataContent();

        foreach (var form in fromForm)
        {
            if (form.Value is string stringValue)
            {
                content.Add(new StringContent(stringValue, Encoding.UTF8), form.Key);
            }
            else
            {
                content.Add(new StringContent(form.Value.Stringify(), Encoding.UTF8), form.Key);
            }
        }

        return content;
    }

    public static async Task<StreamContent> FromFormDataFile(this IFormFile file)
    {
        var length = file.Length;
        await using var fileStream = file.OpenReadStream();

        var memory = new Memory<byte>(new byte[length]);
        _ = await fileStream.ReadAsync(memory).ConfigureAwait(false);

        var fileContent = new StreamContent(new MemoryStream(memory.ToArray()));
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"" + (string.IsNullOrWhiteSpace(file.Name) ? "files" : file.Name) + "\"",
            FileName = "\"" + file.FileName + "\""
        }; // the extra quotes are key here
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        return fileContent;
    }

    public static FormUrlEncodedContent FromFormUrlEncoded(this Dictionary<string, string> fromUrlEncoded) => new(fromUrlEncoded);

    public static Dictionary<string, string> FromHeader(this IHeaderDictionary dictionary) =>
        dictionary.Where(i => !string.IsNullOrWhiteSpace(i.Value))
            .Select(i => new KeyValuePair<string, string>(i.Key, i.Value.ToString()))
            .ToDictionary(t => t.Key, t => t.Value);
}
