namespace Trilha.DotNet.Shared.Extensions;

public static class MailExtensions
{
    public static async Task<bool> SendGridMail(
        this string apiKey
        , EmailAddress from
        , string subject
        , string message
        , List<EmailAddress> to
        , params Attachment[] attachments)
    {
        var client = new SendGridClient(apiKey);

        var msg = new SendGridMessage
        {
            From = from,
            Subject = subject,
            HtmlContent = message
        };

        msg.AddTos(to);
        msg.AddAttachments(attachments);

        var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    public static void SendStpMail(
        this string host
        , NetworkCredential credential
        , MailAddress from
        , string subject
        , string message
        , IEnumerable<string> to
        , bool ssl = false
        , int port = 25
        , params System.Net.Mail.Attachment[] attachments)
    {
        var client = new SmtpClient(host);

        var msg = new MailMessage
        {
            Body = message,
            BodyEncoding = Encoding.UTF8,
            BodyTransferEncoding = TransferEncoding.QuotedPrintable,
            DeliveryNotificationOptions = DeliveryNotificationOptions.None,
            From = from,
            HeadersEncoding = Encoding.UTF8,
            IsBodyHtml = true,
            Priority = MailPriority.High,
            Subject = subject,
            SubjectEncoding = Encoding.UTF8
        };

        foreach (var attachment in attachments)
            msg.Attachments.Add(attachment);

        foreach (var mail in to)
            msg.To.Add(mail);

        client.EnableSsl = ssl;
        client.Port = port;
        client.Credentials = credential;

        client.Send(msg);
    }
}