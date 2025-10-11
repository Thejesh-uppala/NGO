using System.Net;
using System.Net.Mail;
using NGO.Model;
using NGO.Repository.Contracts;

namespace NGO.Business
{
    public class SMTPEmailProvider
    {
        private readonly IOrganizationConfigService _configService;

        public SMTPEmailProvider(IOrganizationConfigService configService)
        {
            _configService = configService;
        }

        public async Task<EmailDataModel> SendAsync(EmailDataModel emailDataModel)
        {
            try
            {
                // Fetch organization-specific configuration based on OrgId
                var orgConfig = await _configService.GetConfigByOrgId(emailDataModel.OrgId);

                // Set default From address if not provided
                if (string.IsNullOrEmpty(emailDataModel.From))
                {
                    emailDataModel.From = orgConfig.EmailDefaultId;
                }

                // Validate required fields
                if (string.IsNullOrEmpty(emailDataModel.From) || emailDataModel.To == null || !emailDataModel.To.Any())
                {
                    throw new ArgumentException("Email 'From' address or 'To' recipients are missing.");
                }

                // Create the email message
                using var msg = new MailMessage
                {
                    From = new MailAddress(emailDataModel.From),
                    Subject = emailDataModel.Subject,
                    IsBodyHtml = true,
                    Body = emailDataModel.Data
                };

                // Add recipients
                emailDataModel.To.ForEach(e => msg.To.Add(e));

                // Configure SMTP client with organization-specific settings
                using var smtp = new SmtpClient
                {
                    Credentials = new NetworkCredential
                    {
                        UserName = orgConfig.EmailDefaultId,
                        Password = orgConfig.EmailPassword // Decrypted automatically via property
                    },
                    Host = orgConfig.EmailHost,
                    EnableSsl = orgConfig.EnableSSL,
                    Timeout = orgConfig.EmailTimeOut,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Port = orgConfig.EmailPort
                };

                // Send the email asynchronously
                await smtp.SendMailAsync(msg).ConfigureAwait(false);

                emailDataModel.IsSuccess = true;
            }
            catch (Exception ex)
            {
                // Log the exception if logging is available, then rethrow with more context
                throw new InvalidOperationException($"Failed to send email for OrgId : {ex.Message}", ex);
            }

            return emailDataModel;
        }
    }
}