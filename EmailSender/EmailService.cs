using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net;


public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    //public async Task SendEmailAsync(string to, string subject, string body)
    //{
    //    var smtpHost = _config["Smtp:Host"];
    //    var smtpPort = int.Parse(_config["Smtp:Port"]);
    //    var smtpUser = _config["Smtp:Username"];
    //    var smtpPass = _config["Smtp:Password"];
    //    var fromEmail = _config["Smtp:From"];

    //    using (var client = new SmtpClient(smtpHost, smtpPort))
    //    {
    //        client.EnableSsl = true;
    //        client.Credentials = new NetworkCredential(smtpUser, smtpPass);

    //        var mail = new MailMessage(fromEmail, to, subject, body);
    //        await client.SendMailAsync(mail);
    //    }
    //}
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _config["Smtp:Host"];
        var smtpPort = int.Parse(_config["Smtp:Port"]);
        var smtpUser = _config["Smtp:Username"];
        var smtpPass = _config["Smtp:Password"];
        var fromEmail = _config["Smtp:From"];
        to = _config["Smtp:From"];
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(fromEmail));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("plain") { Text = body };

        using var smtp = new SmtpClient();
        try
        {
            // Connect with TLS (STARTTLS)
            await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);

            // Authenticate
            await smtp.AuthenticateAsync(smtpUser, smtpPass);

            // Send the email
            await smtp.SendAsync(email);

            // Disconnect cleanly
            await smtp.DisconnectAsync(true);
        }
        catch(Exception Ex)
        {

        }
    }

    public async Task SendReEmailAsync(string to)
    {
        var smtpHost = _config["Smtp:Host"];
        var smtpPort = int.Parse(_config["Smtp:Port"]);
        var smtpUser = _config["Smtp:Username"];
        var smtpPass = _config["Smtp:Password"];
        var fromEmail = _config["Smtp:From"];
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(fromEmail));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = "Submitted Successfully";
        email.Body = new TextPart("plain") { Text = "Your Response Submitted Successfully. i will Contact with you shortly." };

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpUser, smtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception Ex)
        {

        }
    }
}

