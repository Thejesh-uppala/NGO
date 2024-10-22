using NGO.Common;
using NGO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGO.Business
{
    public class ContactUsBusiness
    {
        private readonly SMTPEmailProvider _smtpEmailProvider;

        private readonly AppSettings _appsettings;
        public ContactUsBusiness(SMTPEmailProvider smtpEmailProvider, AppSettings appsettings)
        {
            _smtpEmailProvider = smtpEmailProvider;
            _appsettings = appsettings;
        }

        public async Task SendMail(string contactFormName, string contactFormMessage, string contactFormSubjects, string contactFormEmail)
        {
            var emailDataModel = new EmailDataModel()
            {
                To = new List<string>() {
                    contactFormEmail
                },
                Data = "<p>" + "User created successfully, use the below creadentials to login" + contactFormSubjects + "" + "</p>"
                             + "<p>" + "User Name" + ": " + contactFormName + "</P>"
                             + "<p>" + "Password" + ": " + contactFormMessage + " </P>",
                Subject = "Contact Us"
            };
            await this._smtpEmailProvider.SendAsync(emailDataModel);
        }
    }
}
