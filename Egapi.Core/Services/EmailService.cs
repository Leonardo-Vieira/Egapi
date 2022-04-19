using Egapi.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Egapi.Core.Services
{
    public class EmailService : IEmailService
    {
        private UmbracoHelper _umbraco;
        private IContentService _contentService;
        private ILogger _logger;

        public EmailService(UmbracoHelper umbraco, IContentService contentService, ILogger logger)
        {
            _umbraco = umbraco;
            _contentService = contentService;
            _logger = logger;
        }
        public void SendVerifyEmailAddressNotification(string memberEmail, string verificationToken)
        {
            //gets the template name
            var emailTemplate = GetEmailTemplate("Email Confirmation");

            if (emailTemplate == null)
            {
                throw new Exception("Template not found");
            }

            //email merge the necessary fields
            var subject = emailTemplate.Value<string>("emailTemplateSubjectLine");
            var htmlContent= emailTemplate.Value<string>("emailTemplateHtmlContent");
            var textContent = emailTemplate.Value<string>("emailTemplateTextContent");

            //Mail Merge
            //Mail merge the necessary fields
            //Build the url to be the absolute url to the verify page
            var url = HttpContext.Current.Request.Url.AbsoluteUri
                .Replace(HttpContext.Current.Request.Url.AbsolutePath, String.Empty);
            url += $"/verify?token={verificationToken}";

            MailMerge("link", url, ref htmlContent, ref textContent);

            //send email out to whoever 
            SendMail(memberEmail, subject, htmlContent, textContent);
        }

        private void MailMerge(string token, string url, ref string htmlContent, ref string textContent)
        {
            htmlContent = htmlContent.Replace($"##{token}##", $"<a href='{url}'>Click</a>");
            textContent = textContent.Replace($"##{token}##", url);
        }

        /// <summary>
        /// returns the email template where the title matches the template name
        /// </summary>
        /// <param name="emailTemplate"></param>
        /// <returns></returns>
        private IPublishedContent GetEmailTemplate(string emailTemplate)
        {
            return _umbraco.ContentQuery
                .ContentAtRoot()
                .DescendantsOrSelfOfType("emailTemplate")
                .Where(w => w.Name == emailTemplate)
                .FirstOrDefault();
        }

        private void SendMail(string toAddresses, string subject, string htmlContent, string textContent)
        {
            var siteSettings = _umbraco.ContentAtRoot()
                                      .DescendantsOrSelfOfType("siteSettings")
                                      .FirstOrDefault();
            if (siteSettings == null)
            {
                throw new Exception("There are no site settings");
            }

            var fromAddress = siteSettings.Value<string>("emailSettingsFromAddress");
            if (string.IsNullOrEmpty(fromAddress))
            {
                throw new Exception("There needs to be a from address in site settings");
            }

            //Debug mode
            var debugMode = siteSettings.Value<bool>("testMode");
            var testEmailAccounts = siteSettings.Value<string>("emailTestAccounts");

            var recipients = toAddresses;
            if (debugMode)
            {
                //Change the to - testEmailAccounts
                recipients = testEmailAccounts;
                //Alter subject line - to show in test mode
                subject += " (TEST MODE) - " + toAddresses;
            }

            //Log the email in umbraco 
            //Emails
            //Email
            var emails = _umbraco.ContentAtRoot().DescendantsOrSelfOfType("emails").FirstOrDefault();
            var newEmail = _contentService.Create(toAddresses, emails.Id, "Email");
            newEmail.SetValue("emailSubject", subject);
            newEmail.SetValue("emailToAddress", toAddresses);
            newEmail.SetValue("emailHtmlContent", htmlContent);
            newEmail.SetValue("emailTextContent", textContent);
            newEmail.SetValue("emailSent", false);
            _contentService.SaveAndPublish(newEmail);

            //Send the email via smtp or whatever
            var mimeType = new System.Net.Mime.ContentType("text/html");
            var alternateView = AlternateView.CreateAlternateViewFromString(htmlContent, mimeType);

            var smtpMessage = new MailMessage();
            smtpMessage.AlternateViews.Add(alternateView);

            //To - collection or one email
            foreach (var recipient in recipients.Split(','))
            {
                if (!string.IsNullOrEmpty(recipient))
                {
                    smtpMessage.To.Add(recipient);
                }
            }

            //From
            smtpMessage.From = new MailAddress(fromAddress);

            //Subject
            smtpMessage.Subject = subject;

            //Body
            smtpMessage.Body = textContent;

            //Sending
            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.Send(smtpMessage);
                    newEmail.SetValue("emailSent", true);
                    newEmail.SetValue("emailSentDate", DateTime.Now);
                    _contentService.SaveAndPublish(newEmail);
                }
                catch (Exception exc)
                {
                    //Log the error
                    _logger.Error<EmailService>("There was a problem sending the email", exc);
                    throw exc;
                }
            };
        }
    }
}
