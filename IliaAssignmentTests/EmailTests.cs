using IliaAssignment;
using IliaAssignment.Controllers;
using IliaAssignment.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace IliaAssignmentTests
{
    public class EmailTests
    {
        private IOptions<SMTP> _smtp;
        private IOptions<SMTPInvalido> _smtpInvalido;

        public EmailTests()
        {
            var config = IliaAssignmentTestsInterface.InitConfiguration();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            serviceCollection.Configure<SMTP>(config.GetSection("SMTP"));
            serviceCollection.Configure<SMTPInvalido>(config.GetSection("SMTPInvalido"));
            var services = serviceCollection.BuildServiceProvider();

            _smtp = services.GetService<IOptions<SMTP>>();
            _smtpInvalido = services.GetService<IOptions<SMTPInvalido>>();
        }

        //Faz teste com credenciais inválidas
        [Fact]
        public void CredenciaisInvalidas()
        {
            var emailController = new EmailController(_smtpInvalido);
            Assert.False(emailController.EnviarEmail("Cliente", "cliente@gmail.com", "Assunto", "Corpo"));
        }

        //Faz teste com credenciais válidas
        [Fact]
        public void CredenciaisValidas()
        {
            var emailController = new EmailController(_smtp);
            Assert.True(emailController.EnviarEmail("Cliente", "cliente@gmail.com", "Assunto", "Corpo"));
        }

        //Faz teste sem destinatario
        [Fact]
        public void DestinatarioVazio()
        {
            var emailController = new EmailController(_smtp);
            Assert.False(emailController.EnviarEmail("Cliente", "", "Assunto", "Corpo"));
        }
    }
}
