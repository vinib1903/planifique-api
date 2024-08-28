using PlanifiqueAPI.Core.Interfaces;
using System.Net.Mail;

namespace PlanifiqueAPI.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;

        public EmailService(SmtpClient smtpClient, string fromEmail)
        {
            _smtpClient = smtpClient ?? throw new ArgumentNullException(nameof(smtpClient));
            _fromEmail = fromEmail ?? throw new ArgumentNullException(nameof(fromEmail));
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string name)
        {
            var subject = "Bem-vindo à nossa plataforma!";
            var body = $@"Olá {name},

Obrigado por se cadastrar em nossa plataforma. Estamos felizes em tê-lo conosco!

Atenciosamente,
Equipe Planifique";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail);

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
