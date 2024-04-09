using System.Net;
using System.Net.Mail;
using Azure.Security.KeyVault.Secrets;

namespace intex2.Models
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;
        private static SecretClient _client;
        private string _emailUsername;
        private string _emailPassword;
        private string _emailPort;
        private string _emailHost;
        public EmailHelper(IConfiguration configuration, SecretClient client)
        {
            _configuration = configuration;
            _client = client;
        }
        
        public async Task InitializeAsync()
        {
            _emailUsername = await GetSecretAsync("EmailSettings-Username");
            _emailPassword = await GetSecretAsync("EmailSettings-Password");
            _emailPort = await GetSecretAsync("EmailSettings-Port");
            _emailHost = await GetSecretAsync("EmailSettings-Host");
        }
        
        private async Task<string> GetSecretAsync(string secretName)
        {
            KeyVaultSecret secret = await _client.GetSecretAsync(secretName);
            return secret.Value;
        }
        
        public bool SendEmail(string userEmail, string confirmationLink)
        {
            //var emailSettings = _configuration.GetSection("EmailSettings");
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailUsername),
                Subject = "Confirm your email",
                IsBodyHtml = true,
                Body = confirmationLink
            };
            mailMessage.To.Add(new MailAddress(userEmail));

            using (var client = new SmtpClient(_emailHost, int.Parse(_emailPort)))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_emailUsername, _emailPassword);

                try
                {
                    client.Send(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    // Handle exception
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        public bool SendEmailPasswordReset(string userEmail, string resetLink)
        {
            //var emailSettings = _configuration.GetSection("EmailSettings");
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailUsername), // Use the email configured in user secrets
                To = { new MailAddress(userEmail) },
                Subject = "Password Reset",
                IsBodyHtml = true, // Assuming the reset link is to be embedded in HTML
                Body = "Hello! \n The following link will allow you to confirm your password for ALegoBricks." + resetLink
            };

            using (var client = new SmtpClient(_emailHost, int.Parse(_emailPort)))
            {
                client.EnableSsl = true; // Most SMTP servers require SSL nowadays
                client.Credentials = new NetworkCredential(_emailUsername, _emailPassword);

                try
                {
                    client.Send(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    // Log or handle the exception appropriately
                    Console.WriteLine(ex.Message);
                    return false;
                
                }
            }
        }
        public bool SendEmailTwoFactorCode(string userEmail, string code)
        {
            //var emailSettings = _configuration.GetSection("EmailSettings");
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailUsername), // Configured email as the "From" address
                Subject = "Two Factor Code",
                IsBodyHtml = true,
                Body = $"<p>Your two-factor authentication code is: <strong>{code}</strong></p>"
            };
            mailMessage.To.Add(new MailAddress(userEmail));

            using (var client = new SmtpClient(_emailHost, int.Parse(_emailPort)))
            {
                client.EnableSsl = true; // Assuming SSL is required
                client.Credentials = new NetworkCredential(_emailUsername, _emailPassword);

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