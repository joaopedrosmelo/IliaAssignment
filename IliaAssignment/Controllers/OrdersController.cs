using AutoMapper;
using IliaAssignment.Data;
using IliaAssignment.Models;
using IliaAssignment.Models.DB;
using IliaAssignment.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IliaAssignment.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IOptions<SMTP> _smtp;
        private static IMapper _mapper;
        public OrdersController(ApplicationDBContext context, IOptions<SMTP> smtp, IMapper mapper)
        {
            _context = context;
            _smtp = smtp;
            _mapper = mapper;
        }

        // POST api/<OrdersController>
        [HttpPost]
        public ActionResult Post([FromBody] OrderDTO orderDTO)
        {
            try
            {
                if(!(orderDTO.Price > 0.0m))
                    return BadRequest(JsonConvert.SerializeObject("Price deve ser maior do que 0."));

                var customer = _context.CustomerDBs.Where(c => c.Email == orderDTO.CustomerEmail).FirstOrDefault();
                if (customer == null)
                    return BadRequest(JsonConvert.SerializeObject("Customer não existe."));

                var orderStatus = _context.OrderStatusDBs.Where(o => o.StatusCode == orderDTO.StatusCode).FirstOrDefault();
                if(orderStatus == null)
                    return BadRequest(JsonConvert.SerializeObject("OrderStatus não existe."));

                var orderDB = _mapper.Map<OrdersDB>(orderDTO);
                orderDB.CustomerDB = customer;
                orderDB.IdCustomer = customer.ID;
                orderDB.OrderStatusDB = orderStatus;
                orderDB.CreatedAt = DateTime.Now;
                _context.Add(orderDB);
                _context.SaveChanges();

                var emailController = new EmailController(_smtp);
                string price = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", orderDTO.Price);
                string message = $"Order {orderDB.ID} com o Status - {orderStatus.StatusName} - registrada com sucesso. Valor Total: {price}";

                if (!emailController.EnviarEmail(customer.Name, customer.Email, "Order criada", message))
                    message += message + " *Houve falha ao enviar e-mail de notificação.";

                return Ok(message);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] OrderStatusCodeDTO orderStatusCodeDTO)
        {
            try
            {
                var order = _context.OrdersDBs.Include(o => o.CustomerDB).Where(o => o.ID == id).FirstOrDefault();
                if (order == null)
                    return BadRequest(JsonConvert.SerializeObject("Order não existe."));

                var orderStatus = _context.OrderStatusDBs.Where(o => o.StatusCode == orderStatusCodeDTO.StatusCode).FirstOrDefault();
                if (orderStatus == null)
                    return BadRequest(JsonConvert.SerializeObject("OrderStatus não existe."));

                order.OrderStatusDB = orderStatus;
                order.IdStatus = orderStatusCodeDTO.StatusCode;
                _context.Update(order);
                _context.SaveChanges();

                var emailController = new EmailController(_smtp);
                string price = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", order.Price);
                string message = $"Order {id} com novo Status - {orderStatus.StatusName}";
                
                if(!emailController.EnviarEmail(order.CustomerDB.Name, order.CustomerDB.Email, "Status da Order atualizada", message))
                    message += message + " *Houve falha ao enviar e-mail de notificação.";

                return Ok(message);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
