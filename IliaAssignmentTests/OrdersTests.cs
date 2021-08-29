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
    public class OrdersTests
    {
        private ApplicationDBContext context;
        private IMapper mapper;
        public IConfiguration Configuration { get; }

        //Faz teste com Customer inexistente
        [Fact]
        public void CadastroOrderCustomerInexistente()
        {
            IniciaDependenciaInMemoryDatabase();

            var orderController = new OrdersController(context, mapper);

            var order = new OrderDTO()
            {
                Price = 100,
                StatusCode = 0,
                CustomerEmail = "cliente@naoexiste.com"
            };

            var response = orderController.Post(order);

            response.Should().BeOfType<BadRequestObjectResult>()
                .Which.StatusCode.Should().Be((int)(HttpStatusCode.BadRequest));
        }

        //Faz teste com OrderStatus inexistente
        [Fact]
        public void CadastroOrderStatusInexistente()
        {
            IniciaDependenciaInMemoryDatabase();

            var customerController = new CustomersController(context, mapper);

            var customer = new CustomerDTO()
            {
                Name = "Joao",
                Email = "joao@teste.com"
            };

            customerController.Post(customer);

            var orderController = new OrdersController(context, mapper);

            var order = new OrderDTO()
            {
                Price = 100,
                StatusCode = 5,
                CustomerEmail = "joao@teste.com"
            };

            var response = orderController.Post(order);

            response.Should().BeOfType<BadRequestObjectResult>()
                .Which.StatusCode.Should().Be((int)(HttpStatusCode.BadRequest));
        }

        //Faz teste com Price inválido
        [Theory]
        [InlineData(-1, 0, "Joao", "joao@teste.com")]
        [InlineData(0, 0, "Joao", "joao@teste.com")]
        public void CadastroOrderPriceInvalido(decimal Price, int StatusCode, string CustomerName, string CustomerEmail)
        {
            IniciaDependenciaInMemoryDatabase();

            var customerController = new CustomersController(context, mapper);

            var customer = new CustomerDTO()
            {
                Name = CustomerName,
                Email = CustomerEmail
            };

            customerController.Post(customer);

            var orderController = new OrdersController(context, mapper);

            var order = new OrderDTO()
            {
                Price = Price,
                StatusCode = StatusCode,
                CustomerEmail = CustomerEmail
            };

            var response = orderController.Post(order);

            response.Should().BeOfType<BadRequestObjectResult>()
                .Which.StatusCode.Should().Be((int)(HttpStatusCode.BadRequest));
        }

        //Faz teste com Orders válidas
        [Theory]
        [InlineData(1, 0, "Joao", "joao@teste.com")]
        [InlineData(0.5, 0, "Joao", "joao@teste.com")]
        [InlineData(100, 0, "Joao", "joao@teste.com")]
        public void CadastroOrderPriceValido(decimal Price, int StatusCode, string CustomerName, string CustomerEmail)
        {
            IniciaDependenciaInMemoryDatabase();

            context.OrderStatusDBs.Add(new IliaAssignment.Models.DB.OrderStatusDB
            {
                StatusCode = 0,
                StatusName = "Aguardando Pagamento"
            });

            var customerController = new CustomersController(context, mapper);

            var customer = new CustomerDTO()
            {
                Name = CustomerName,
                Email = CustomerEmail
            };

            customerController.Post(customer);

            var orderController = new OrdersController(context, mapper);

            var order = new OrderDTO()
            {
                Price = Price,
                StatusCode = StatusCode,
                CustomerEmail = CustomerEmail
            };

            var response = orderController.Post(order);

            response.Should().BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be((int)(HttpStatusCode.OK));
        }

        [Fact]
        public void ErroDeConexadoBancoDeDados()
        {
            IniciaDependenciaDatabase("server=remotemysql.;uid=0pqPBpePtn;password=JwJG6Wrivv;database=0pqPBpePtn");
            
            var orderController = new OrdersController(context, mapper);

            var order = new OrderDTO()
            {
                Price = 1000,
                StatusCode = 0,
                CustomerEmail = "joao@teste.com"
            };

            var response = orderController.Post(order);

            response.Should().BeOfType<StatusCodeResult>()
                .Which.StatusCode.Should().Be((int)(HttpStatusCode.InternalServerError));
        }

        //Inicia classes utilizadas como injeção de dependência para o Controller Order
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
