using System.Net;
using System.Net.Mail;
 
namespace intex2.Models
{
    public class EmailHelper
    {
        public bool SendEmail(string userEmail, string confirmationLink)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("ASalesLego@outlook.com"),
                Subject = "Confirm your email",
                IsBodyHtml = true,
                Body = confirmationLink
            };
            mailMessage.To.Add(new MailAddress(userEmail));

            using (var client = new SmtpClient("smtp-mail.outlook.com", 587))
            {
                client.EnableSsl = true; // Use SSL for security
                client.Credentials = new NetworkCredential("ASalesLego@outlook.com", "mk9XaFeFhscz44");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                try
                {
                    client.Send(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    // Log or handle the exception
                    return false;
                }
            }
        }
        public bool SendEmailPasswordReset(string userEmail, string resetLink)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("ASalesLego@outlook.com"), // Your Outlook email address
                Subject = "Password Reset",
                IsBodyHtml = true, // If your link is embedded in HTML
                Body = resetLink
            };
            mailMessage.To.Add(new MailAddress(userEmail));

            using (var client = new SmtpClient("smtp-mail.outlook.com", 587)) // SMTP server for Outlook.com
            {
                client.EnableSsl = true; // SSL needs to be enabled
                client.Credentials = new NetworkCredential("ASalesLego@outlook.com.com", "mk9XaFeFhscz44"); // Use your Outlook credentials
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                try
                {
                    client.Send(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    // Log or handle the exception appropriately
                    return false;
                }
            }
        }
    }
}