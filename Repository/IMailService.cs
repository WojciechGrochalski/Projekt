using System;

namespace AngularApi.Repository
{
    public interface IMailService
    {
        void SendMail(string to, string subject, string html);
        Tuple<string, string> GenerateMessageForUserVerification(string href, string token);
    }
}
