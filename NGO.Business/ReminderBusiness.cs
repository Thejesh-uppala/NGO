using System.Threading.Channels;
using NGO.Common;
using NGO.Model;
using NGO.Repository;

namespace NGO.Business
{
    public class ReminderBusiness
    {
        private readonly SMTPEmailProvider _smtpEmailProvider;

        private readonly AppSettings _appsettings;
        private readonly IUserRepository _userRepository;
        private readonly Channel<EmailDataModel> _emailChannel;

        private readonly CommonBusiness _commonBusiness;
        private readonly UserBusiness _userBusiness;

        public ReminderBusiness(Channel<EmailDataModel> emailChannel, UserBusiness userBusiness, SMTPEmailProvider smtpEmailProvider, AppSettings appsettings, IUserRepository userRepository, CommonBusiness commonBusiness)
        {
            _smtpEmailProvider = smtpEmailProvider;
            _appsettings = appsettings;
            _userRepository = userRepository;
            _emailChannel = emailChannel;
            _commonBusiness = commonBusiness;
            _userBusiness = userBusiness;
        }

        public async Task SendMail(string name, string message, string subject, string email, int orgId)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(message) ||
                string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(email) || orgId <= 0)
            {
                throw new ArgumentException("All fields including orgId are required");
            }
            var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
          await _userBusiness.GetBrandDetailsAsync(orgId);

            string emailTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync(
                "user",
                "contact-us-response.html", orgId
            );


            string emailBody = emailTemplate
                .Replace("{Name}", name)
                .Replace("{Message}", message)
                .Replace("{Subject}", subject)
                .Replace("{OrgName}", orgName)
                .Replace("{PrimaryEmail}", primaryEmail);

            var emailDataModel = new EmailDataModel
            {
                To = new List<string> { email },
                Data = emailBody,
                Subject = $"Thank You for Contacting Us for: {subject}",
                OrgId = orgId
            };
            await _emailChannel.Writer.WriteAsync(emailDataModel);

            //admin email
            string adminEmailTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync(
               "admin",
               "contact-us-notify.html", orgId
            );

            string adminEmailBody = adminEmailTemplate
                .Replace("{Name}", name)
                .Replace("{Message}", message)
                .Replace("{Subject}", subject)
                .Replace("{Email}", email)
                .Replace("{OrgName}", orgName);


            var adminEmailDataModel = new EmailDataModel
            {
                To = new List<string> { primaryEmail },
                Data = adminEmailBody,
                Subject = $"New Contact Form Submission: {subject} (Org: {orgName})",
                OrgId = orgId
            };

            await _emailChannel.Writer.WriteAsync(adminEmailDataModel);
        }


        public async Task SendReminderMail(List<string> userEmails, int updatedBy, int orgId)
        {
            string emailBodyTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("user", "membership-reminder.html", orgId);

            // Path to the email template


            var users = await _userRepository.GetUsersByEmailsAndOrg(userEmails, orgId);
            if (users == null || !users.Any())
            {
                throw new Exception("No users found for the provided emails and organization ID.");
            }
            var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
           await _userBusiness.GetBrandDetailsAsync(orgId);


            foreach (var user in users)
            {
                if (user == null) continue;

                var membershipExpiryDate = user.MembershipValidityDate?.ToString("dd MMM yyyy") ?? "N/A";
                var isExpired = user.MembershipValidityDate.HasValue && user.MembershipValidityDate.Value < DateTime.UtcNow;

                var emailBody = emailBodyTemplate
                    .Replace("{FirstName}", user.FirstName)
                    .Replace("{LastName}", user.LastName)
                    .Replace("{SupportEmail}", primaryEmail)
                    .Replace("{orgName}", orgName)
                    .Replace("{MembershipExpiryDate}", membershipExpiryDate)
                    .Replace("{ExpiryMessage}", isExpired
                        ? "Unfortunately, your membership has expired on"
                        : "This is a gentle reminder that your membership is set to expire on");

                var emailDataModel = new EmailDataModel
                {
                    To = new List<string> { user.Email },
                    Data = emailBody,
                    Subject = isExpired ? "Membership Expired Notification" : "Membership Validity Reminder",
                    OrgId = orgId
                };
                await _emailChannel.Writer.WriteAsync(emailDataModel);
            }


        }
    }


}
