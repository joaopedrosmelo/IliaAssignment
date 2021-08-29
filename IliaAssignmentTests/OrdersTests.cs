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
using Microsoft.Extensions.Options;
using IliaAssignment.Models;
using Microsoft.Extensions.DependencyInjection;
using IliaAssignment.Models.DB;

namespace IliaAssignmentTests
{
    public class OrdersTests
    {
        private ApplicationDBContext _context;
        private IOptions<SMTP> _smtp;
        private string _connectionString;
        private IMapper _mapper;
        public IConfiguration Configuration { get; }

        public OrdersTests()
        {
            var config = IliaAssignmentTestsInterface.InitConfiguration();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            serviceCollection.Configure<SMTP>(config.GetSection("SMTP"));
            var services = serviceCollection.BuildServiceProvider();
            
            _smtp = services.GetService<IOptions<SMTP>>();
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        //Faz teste com Customer inexistente
        [Fact]
        public void CadastroOrderCustomerInexistente()
        {
            IniciaDependenciaInMemoryDatabase();

            var orderController = new OrdersController(_context, _smtp, _mapper);

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

            var customerController = new CustomersController(_context, _mapper);

            var customer = new CustomerDTO()
            {
                Name = "Joao",
                Email = "joao@teste.com"
            };

            customerController.Post(customer);

            var orderController = new OrdersController(_context, _smtp, _mapper);

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

            var customerController = new CustomersController(_context, _mapper);

            var customer = new CustomerDTO()
            {
                Name = CustomerName,
                Email = CustomerEmail
            };

            customerController.Post(customer);

            var orderController = new OrdersController(_context, _smtp, _mapper);

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

        //Faz teste com Orders válidas e incluindo mesmo Customer
        [Theory]
        [InlineData(100, 0, "Aguardando Pagamento", "Joao", "joaopedrosmelo@gmail.com")]
        [InlineData(1, 0, "Aguardando Pagamento", "Joao", "joao@teste.com")]
        [InlineData(0.5, 0, "Aguardando Pagamento", "Joao", "joao@teste.com")]
        [InlineData(100, 0, "Aguardando Pagamento", "Joao", "joao@teste.com")]
        public void CadastroOrderValida(decimal Price, int StatusCode, string StatusName, string CustomerName, string CustomerEmail)
        {
            IniciaDependenciaInMemoryDatabase();

            _context.OrderStatusDBs.Add(new OrderStatusDB
            {
                StatusCode = StatusCode,
                StatusName = StatusName
            });

            var customerController = new CustomersController(_context, _mapper);

            var customer = new CustomerDTO()
            {
                Name = CustomerName,
                Email = CustomerEmail
            };

            customerController.Post(customer);

            var orderController = new OrdersController(_context, _smtp, _mapper);

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
            IniciaDependenciaDatabase();
            
            var orderController = new OrdersController(_context, _smtp, _mapper);

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
