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

namespace IliaAssignmentTests
{
    public class CustomersTests
    {
        private ApplicationDBContext context;
        private IMapper mapper;
        public IConfiguration Configuration { get; }

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

            var customerController = new CustomersController(context, mapper);

            var customer = new Customer()
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

            var customerController = new CustomersController(context, mapper);

            var customer = new Customer()
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

            var customerController = new CustomersController(context, mapper);

            for (var i = 0; i < 2; i++)
            {
                var customer = new Customer()
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
            IniciaDependenciaDatabase("server=remotemysql.;uid=0pqPBpePtn;password=JwJG6Wrivv;database=0pqPBpePtn");
            var customerController = new CustomersController(context, mapper);

            var customer = new Customer()
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
            context = new ApplicationDBContext(options);

            IniciaMapeamento();
        }
        private void IniciaDependenciaDatabase(string connectionString)
        {
            //Inicia banco de dados 'temporário' para não afetar ambiente de produção
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseMySql(connectionString)
                .Options;
            context = new ApplicationDBContext(options);

            IniciaMapeamento();
        }
        private void IniciaMapeamento()
        {
            //Cria mapeamento baseado na classe Profiles
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Profiles());
            });
            mapper = config.CreateMapper();
        }
    }
}
