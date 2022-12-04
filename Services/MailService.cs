using AngularApi.Repository;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;

namespace angularapi.Services
{
    public class MailService : IMailService
    {
        private readonly AppSettings _appSettings;

        public MailService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public void SendMail(string to, string subject, string html)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("cashdataa@op.pl"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(_appSettings.SmtpUser, _appSettings.SmtpPass);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
        public Tuple<string, string> GenerateMessageForUserVerification(string href, string token)
        {
            string subject = "Sign-up Verification API - Verify Email";
            string message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                             <p> <a href=""{href}verify-user?token={token}""> link <a/> </p>";
            return new Tuple<string, string>(subject, message);
        }
    }
}
