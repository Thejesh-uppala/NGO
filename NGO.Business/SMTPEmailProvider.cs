using NGO.Common;
using NGO.Model;
using System.Net;
using System.Net.Mail;

namespace NGO.Business
{
    public class SMTPEmailProvider
    {
        private readonly EmailSettings _emailSettings;

        public SMTPEmailProvider(EmailSettings emailSettings)
        {
            this._emailSettings = emailSettings;
        }

        public async Task<EmailDataModel> SendAsync(EmailDataModel emailDataModel)
        {
            try
            {
                if (string.IsNullOrEmpty(emailDataModel.From))
                {
                    emailDataModel.From = this._emailSettings.EmailDefaultId;
                }
                var msg = new MailMessage();
                msg.From = (new MailAddress(emailDataModel.From));
                emailDataModel.To.ForEach(e =>
                {
                    msg.To.Add(e);
                });
                msg.Subject = emailDataModel.Subject;
                msg.IsBodyHtml = true;
                msg.Body = emailDataModel.Data;
                using (var smtp = new SmtpClient())
                {
                    smtp.Credentials = new NetworkCredential()
                    {
                        UserName = this._emailSettings.EmailDefaultId,
                        Password = this._emailSettings.EmailPassword
                    };
                    smtp.Host = this._emailSettings.EmailHost;
                    smtp.EnableSsl = this._emailSettings.EnableSSL;
                    smtp.Timeout = this._emailSettings.EmailTimeOut;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Port = this._emailSettings.EmailPort;

                    await smtp.SendMailAsync(msg).ConfigureAwait(false);
                }
                emailDataModel.IsSuccess = true;
            }
            catch
            {
                throw;
            }
            return emailDataModel;
        }
    }
}
