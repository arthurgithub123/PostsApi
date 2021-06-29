using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace PostsApi.Models.Email
{
    public class EmailHelper
    {
        public static void SendEmail(string fromName, string fromAddress, string toName, string toAddress, string subject, string htmlBody, string senderEmail, string senderPassword)
        {
			var message = new MimeMessage();
		
			message.From.Add(new MailboxAddress(fromName, fromAddress));
			message.To.Add(new MailboxAddress(toName, toAddress));
			message.Subject = subject;

			message.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

			using (var smtpClient = new SmtpClient())
			{
				smtpClient.CheckCertificateRevocation = false;

				smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

				smtpClient.Authenticate(senderEmail, senderPassword);

				smtpClient.Send(message);
				smtpClient.Disconnect(true);
			}
		}
    }
}
