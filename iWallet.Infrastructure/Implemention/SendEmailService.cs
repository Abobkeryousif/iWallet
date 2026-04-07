namespace iWallet.Infrastructure.Implemention
{
    public class SendEmailService : ISendEmailService
    {
        private readonly MailSetting _setting;
        public SendEmailService(IOptions<MailSetting> setting)
        {
            _setting = setting.Value;
        }
        //public void SendEmail(string MailTo, string subject, string message)
        //{
        //    using (var client = new SmtpClient())
        //    {
        //        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

        //        client.Connect(_setting.Host, _setting.Port);
        //        client.Authenticate(_setting.Email, _setting.Password);

        //        var bodyBuilder = new BodyBuilder
        //        {
        //            HtmlBody = message,
        //            TextBody = "hello"
        //        };

        //        var Message = new MimeMessage
        //        {
        //            Body = bodyBuilder.ToMessageBody()
        //        };

        //        Message.From.Add(new MailboxAddress("iWallet Team", _setting.Email));
        //        Message.To.Add(new MailboxAddress("Hello", MailTo));
        //        Message.Subject = subject;
        //        client.Send(Message);
        //        client.Disconnect(true);
        //    }
        //}
    }
}
