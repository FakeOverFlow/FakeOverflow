using System.Collections.Concurrent;
using FakeoverFlow.Backend.Abstraction;
using FakeoverFlow.Backend.Http.Api.Abstracts.Clients;
using FakeoverFlow.Backend.Http.Api.Utils;
using MailKit.Net.Smtp;
using MimeKit;

namespace FakeoverFlow.Backend.Http.Api.Clients;

public class EmailClient : IEmailClient
{
    
    private readonly ConcurrentDictionary<string, string> _emailTemplates = new();
    private readonly string _host;
    private readonly string _port;
    private readonly string _username;
    private readonly string _password;
    private readonly string _from;

    public EmailClient(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SmtpConnectionString")!;
        var clientConnection = connectionString.SplitAndParseFromString();
        _host = clientConnection["host"];
        _port = clientConnection["port"] ?? "587";
        _username = clientConnection["username"];
        _password = clientConnection["password"];
        _from = clientConnection["from"];
    }
    
    public async Task<bool> SendEmailAsync(IEmailTemplate message, string[]? toList = null, string[]? ccList = null,
        string[]? bccList = null, Dictionary<string, string>? placeholders = null)
    {
        if (toList == null && ccList == null && bccList == null)
        {
            return false;
        }
        
        var mimeMessage = new MimeMessage();
        if (toList != null)
        {
            foreach (var se in toList)
            {
                mimeMessage.To.Add(MailboxAddress.Parse(se));
            }
        }

        if (ccList != null)
        {
            foreach (var se in ccList)
            {
                mimeMessage.Cc.Add(MailboxAddress.Parse(se));
            }
        }

        if (bccList != null)
        {
            foreach (var se in bccList)
            {
                mimeMessage.Bcc.Add(MailboxAddress.Parse(se));
            }
        }
        
        mimeMessage.From.Add(MailboxAddress.Parse(_from));
        
        mimeMessage.Subject = message.asSubject(placeholders);
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message.AsMailTemplate(placeholders)
        };
        mimeMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_host, int.Parse(_port), false);
        await client.AuthenticateAsync(_username, _password);
        await client.SendAsync(mimeMessage);
        await client.DisconnectAsync(true);
        return true;
    }

    public Task<bool> SendEmailAsync(string subject, string message, string[]? toList = null, string[]? ccList = null,
        string[]? bccList = null, Dictionary<string, string>? placeholders = null)
    {
        throw new NotImplementedException();
    }
}