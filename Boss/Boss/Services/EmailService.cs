using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public static class EmailService
{




    public class EmailConfig
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string FromEmail { get; set; }
        public string Password { get; set; }
    }

    public static class EmailConfigurationLoader
    {
        public static EmailConfig LoadEmailConfig()
        {
            var config = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("appsettings.json"));
            return JsonConvert.DeserializeObject<EmailConfig>(config.EmailSettings.ToString());
        }
    }

    public static void SendEmail(string toEmail, string subject, string body)
    {
        try
        {
            var emailConfig = EmailConfigurationLoader.LoadEmailConfig();

            var smtpClient = new SmtpClient(emailConfig.SmtpServer)
            {
                Port = emailConfig.Port,
                Credentials = new NetworkCredential(emailConfig.FromEmail, emailConfig.Password),
                EnableSsl = emailConfig.EnableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailConfig.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);

            Console.WriteLine($"Təsdiq e-poçtu {toEmail} ünvanına göndərildi.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"E-poçt göndərmə xətası: {ex.Message}");
        }
    }
    
}
