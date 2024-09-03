using PlanifiqueAPI.Core.Interfaces;
using System.Net.Mail;

namespace PlanifiqueAPI.Application.Services
{
    public class EmailService : IEmailService // implementação da interface
    {

        // injeção de dependências
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;

        public EmailService(SmtpClient smtpClient, string fromEmail)
        {
            _smtpClient = smtpClient ?? throw new ArgumentNullException(nameof(smtpClient));
            _fromEmail = fromEmail ?? throw new ArgumentNullException(nameof(fromEmail));
        }

        // método para enviar um email de boas-vindas aos novos usuários
        public async Task SendWelcomeEmailAsync(string toEmail, string name)
        {
            // assunto
            var subject = "Bem-vindo à nossa plataforma!";

            // corpo da mensagem
            var body = $@"<html>
                    <body>
                        <p>Olá {name},</p>
                        <p>Obrigado por se cadastrar em nossa plataforma. Estamos felizes em tê-lo conosco!</p>
                        <img src=""https://files.oaiusercontent.com/file-EuD0lgXAMSplOQwostAikyju?se=2024-09-03T00%3A21%3A06Z&sp=r&sv=2024-08-04&sr=b&rscc=max-age%3D604800%2C%20immutable%2C%20private&rscd=attachment%3B%20filename%3De0042571-7c55-43c4-840f-b4d2add51646.webp&sig=PsQFtaqpgKu%2BV%2BRRkwDNb%2BWOHXZeeMPKhWo1JEhaa9c%3D"" alt=""Imagem de boas-vindas"" />
                        <p>Atenciosamente,<br>Equipe Planifique</p>
                    </body>
                  </html>";

            // monta o email com as variáveis
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            // captura o email do usuário no momento da criação para o envio do email
            mailMessage.To.Add(toEmail);

            // envia o email usando o SMTP
            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
