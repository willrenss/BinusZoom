using System.Net.Mail;

namespace BinusZoom.Service.EmailService;

public class MailSender
{
    private readonly MailSettings _mailSettings;
    private readonly SmtpClient _smtpClient;
    public MailSender(MailSettings mailSettings)
    {
        _mailSettings = mailSettings;
        _smtpClient = new SmtpClient(_mailSettings.Server, _mailSettings.Port)
        {
            Credentials = null,
            EnableSsl = false
        };
    }

    public async Task<bool> SendMail(MailData mailData)
    {
        using (MailMessage message = new MailMessage())
        {
            message.From = new MailAddress(_mailSettings.SenderEmail, _mailSettings.SenderName);
            message.To.Add(new MailAddress(mailData.EmailToId, mailData.EmailToName));
            message.Subject = mailData.EmailSubject;
            message.Body = mailData.EmailBody;
            message.IsBodyHtml = true;

            await _smtpClient.SendMailAsync(message, CancellationToken.None);
            return true;
        }
    }
    
    public async Task<bool> SendMailWithAttachment(MailData mailData, byte[] attachmentByte)
    {
        using (MailMessage message = new MailMessage())
        {
            message.From = new MailAddress(_mailSettings.SenderEmail, _mailSettings.SenderName);
            message.To.Add(new MailAddress(mailData.EmailToId, mailData.EmailToName));
            message.Subject = mailData.EmailSubject;
            message.Body = mailData.EmailBody;
            message.IsBodyHtml = true;
            
            
            using (MemoryStream ms = new MemoryStream(attachmentByte))
            {
                message.Attachments.Add(new Attachment(ms, "Certificate", "application/pdf"));
                using (SmtpClient smtp = new SmtpClient(_mailSettings.Server, _mailSettings.Port))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(_mailSettings.UserName, _mailSettings.Password);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message, CancellationToken.None);
                    return true;
                }
            }
        }
    }
}