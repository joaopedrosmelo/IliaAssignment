using Xunit;
using IliaAssignment.Models.DB;
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
using System.Linq;
using Microsoft.Extensions.Options;
using IliaAssignment.Models;
using Microsoft.Extensions.DependencyInjection;

namespace IliaAssignmentTests
{
    public class OrderStatusTest
    {
        private ApplicationDBContext context;
        private IOptions<SMTP> _smtp;
        private string _connectionString;
        private IMapper mapper;
        public IConfiguration Configuration { get; }

        public OrderStatusTest()
        {
            var config = IliaAssignmentTestsInterface.InitConfiguration();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            serviceCollection.Configure<SMTP>(config.GetSection("SMTP"));
            var services = serviceCollection.BuildServiceProvider();

            _smtp = services.GetService<IOptions<SMTP>>();
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        //Faz teste com Order inexistente
        [Fact]
        public void AtualizaStatusOrderInexistente()
        {
            IniciaDependenciaInMemoryDatabase();

            var orderController = new OrdersController(context, _smtp, mapper);
            
            var order = new OrderStatusCodeDTO()
            {
                StatusCode = 0
            };

            var response = orderController.Put(999999999, order);

            response.Should().BeOfType<BadRequestObjectResult>()
                .Which.StatusCode.Should().Be((int)(HttpStatusCode.BadRequest));
        }

        //Faz teste com Status inexistente
        [Fact]
        public void AtualizaStatusInexistente()
        {
            IniciaDependenciaInMemoryDatabase();

            context.OrderStatusDBs.Add(new OrderStatusDB
            {
                StatusCode = 0,
                StatusName = "Aguardando Pagamento"
            });

            var customerController = new CustomersController(context, mapper);

            var customer = new CustomerDTO()
            {
                Name = "Joao",
                Email = "joao@teste.com"
            };

            customerController.Post(customer);

            var orderController = new OrdersController(context, _smtp, mapper);

            var order = new OrderDTO()
            {
                Price = 100,
                StatusCode = 0,
                CustomerEmail = "joao@teste.com"
            };

            orderController.Post(order);

            var orderStatus = new OrderStatusCodeDTO()
            {
                StatusCode = 9999
            };
            
            var response = orderController.Put(context.OrdersDBs.FirstOrDefault().ID, orderStatus);

            response.Should().BeOfType<BadRequestObjectResult>()
                .Which.StatusCode.Should().Be((int)(HttpStatusCode.BadRequest));
        }

        //Faz teste com Order e Status validos
        [Fact]
        public void AtualizaOrderStatusExistentes()
        {
            int StatusCode = 0;
            int StatusCodeUpdate = 1;

            IniciaDependenciaInMemoryDatabase();

            context.OrderStatusDBs.Add(new OrderStatusDB
            {
                StatusCode = StatusCode,
                StatusName = "Aguardando Pagamento"
            });

            context.OrderStatusDBs.Add(new OrderStatusDB
            {
                StatusCode = StatusCodeUpdate,
                StatusName = "Pedido em Preparação"
            });

            var customerController = new CustomersController(context, mapper);

            var customer = new CustomerDTO()
            {
                Name = "Joao",
                Email = "joao@teste.com"
            };

            customerController.Post(customer);

            var orderController = new OrdersController(context, _smtp, mapper);

            var order = new OrderDTO()
            {
                Price = 100,
                StatusCode = StatusCode,
                CustomerEmail = "joao@teste.com"
            };

            orderController.Post(order);

            var orderStatus = new OrderStatusCodeDTO()
            {
                StatusCode = StatusCodeUpdate
            };

            var response = orderController.Put(context.OrdersDBs.FirstOrDefault().ID, orderStatus);

            response.Should().BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be((int)(HttpStatusCode.OK));
        }

        [Fact]
        public void ErroDeConexadoBancoDeDados()
        {
            IniciaDependenciaDatabase();

            var orderController = new OrdersController(context, _smtp, mapper);

            var order = new OrderStatusCodeDTO()
            {
                StatusCode = 0
            };

            var response = orderController.Put(0, order);

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
        private void IniciaDependenciaDatabase()
        {
            //Inicia banco de dados 'temporário' para não afetar ambiente de produção
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseMySql(_connectionString)
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
