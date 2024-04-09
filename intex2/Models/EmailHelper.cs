using System.Net;
using System.Net.Mail;
 
namespace intex2.Models
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;
        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool SendEmail(string userEmail, string confirmationLink)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["Username"]),
                Subject = "Confirm your email",
                IsBodyHtml = true,
                Body = confirmationLink
            };
            mailMessage.To.Add(new MailAddress(userEmail));

            using (var client = new SmtpClient(emailSettings["Host"], int.Parse(emailSettings["Port"])))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]);

                try
                {
                    client.Send(mailMessage);
                    return true;
                }
                catch (Exception)
                {
                    // Handle exception
                    return false;
                }
            }
        }
        public bool SendEmailPasswordReset(string userEmail, string resetLink)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["Username"]), // Use the email configured in user secrets
                To = { new MailAddress(userEmail) },
                Subject = "Password Reset",
                IsBodyHtml = true, // Assuming the reset link is to be embedded in HTML
                Body = resetLink
            };

            using (var client = new SmtpClient(emailSettings["Host"], int.Parse(emailSettings["Port"])))
            {
                client.EnableSsl = true; // Most SMTP servers require SSL nowadays
                client.Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]);

                try
                {
                    client.Send(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    // Log or handle the exception appropriately
                    // For example: Console.WriteLine(ex.Message);
                    return false;
                
                }
            }
        }
    }
}