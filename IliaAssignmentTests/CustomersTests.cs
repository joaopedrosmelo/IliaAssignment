using Xunit;
using IliaAssignment.Models.DTO;
using IliaAssignment.Controllers;
using Microsoft.EntityFrameworkCore;
using IliaAssignment.Data;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using IliaAssignment.Profiles;
using Microsoft.Extensions.Configuration;
using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace IliaAssignmentTests
{
    public class CustomersTests
    {
        private ApplicationDBContext _context;
        private string _connectionString;
        private IMapper _mapper;
        public IConfiguration Configuration { get; }

        public CustomersTests()
        {
            var config = IliaAssignmentTestsInterface.InitConfiguration();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();

            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        //Faz testes com nome ou e-mail inválidos para registro
        [Theory]
        [InlineData("Dummy", "emailinvalido")]
        [InlineData("Dummy", "emailinvalido@")]
        [InlineData("Dummy", "emailinvalido.com")]
        [InlineData("", "emailinvalido@gmail")]
        [InlineData("", "emailinvalido@gmail.")]
        [InlineData("", "emailinvalido@gmail.com")]
        public void CadastroCustomerNomeEmailInvalido(string Name, string Email)
        {
            IniciaDependenciaInMemoryDatabase();

            var customerController = new CustomersController(_context, _mapper);

            var customer = new CustomerDTO()
            {
                Name = Name,
                Email = Email
            };

            var response = customerController.Post(customer);

            response.Should().BeOfType<BadRequestObjectResult>()
                .Which.StatusCode.Should().Be((int)(HttpStatusCode.BadRequest));
        }

        //Faz testes com nome ou e-mail válidos para registro
        [Theory]
        [InlineData("Jose Roberto", "emailinvalido@gmail.com")]
        [InlineData("Alberto", "emailinvalido@ilia.digital")]
        [InlineData("Carlos Almeida", "emailinvalido@hotmail.com")]
        public void CadastroCustomerNomeEmailValido(string Name, string Email)
        {
            IniciaDependenciaInMemoryDatabase();

            var customerController = new CustomersController(_context, _mapper);

            var customer = new CustomerDTO()
            {
                Name = Name,
                Email = Email
            };

            var response = customerController.Post(customer);

            response.Should().BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be((int)(HttpStatusCode.OK));
        }

        [Fact]
        public void CadastroCustomerJaExistente()
        {
            IniciaDependenciaInMemoryDatabase();

            var customerController = new CustomersController(_context, _mapper);

            for (var i = 0; i < 2; i++)
            {
                var customer = new CustomerDTO()
                {
                    Name = "joao",
                    Email = "joao@teste.com"
                };

                var response = customerController.Post(customer);
                if(i == 1)
                    response.Should().BeOfType<BadRequestObjectResult>()
                        .Which.StatusCode.Should().Be((int)(HttpStatusCode.BadRequest));
            }
        }

        [Fact]
        public void ErroDeConexadoBancoDeDados()
        {
            IniciaDependenciaDatabase();
            var customerController = new CustomersController(_context, _mapper);

            var customer = new CustomerDTO()
            {
                Name = "Joao",
                Email = "joao@teste.com"
            };

            var response = customerController.Post(customer);

            response.Should().BeOfType<StatusCodeResult>()
                .Which.StatusCode.Should().Be((int)(HttpStatusCode.InternalServerError));
        }

        //Inicia classes utilizadas como injeção de dependência para o Controller Customer
        private void IniciaDependenciaInMemoryDatabase()
        {
            //Inicia banco de dados 'temporário' para não afetar ambiente de produção
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase("ApplicationDBContext")
                .Options;
            _context = new ApplicationDBContext(options);

            IniciaMapeamento();
        }
        private void IniciaDependenciaDatabase()
        {
            //Inicia banco de dados 'temporário' para não afetar ambiente de produção
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseMySql(_connectionString)
                .Options;
            _context = new ApplicationDBContext(options);

            IniciaMapeamento();
        }
        private void IniciaMapeamento()
        {
            //Cria mapeamento baseado na classe Profiles
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Profiles());
            });
            _mapper = config.CreateMapper();
        }
    }
}
