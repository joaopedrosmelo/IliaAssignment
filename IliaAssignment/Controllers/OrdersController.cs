using AutoMapper;
using IliaAssignment.Data;
using IliaAssignment.Models.DB;
using IliaAssignment.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IliaAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private static IMapper _mapper;
        public OrdersController(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
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

                var clienteExistente = _context.CustomerDBs.Where(c => c.Email == orderDTO.CustomerEmail).FirstOrDefault();
                if (clienteExistente == null)
                    return BadRequest(JsonConvert.SerializeObject("Customer não existe."));

                var orderStatus = _context.OrderStatusDBs.Where(o => o.StatusCode == orderDTO.StatusCode).FirstOrDefault();
                if(orderStatus == null)
                    return BadRequest(JsonConvert.SerializeObject("OrderStatus não existe."));

                var orderDB = _mapper.Map<OrdersDB>(orderDTO);
                orderDB.CustomerDB = clienteExistente;
                orderDB.IdCustomer = clienteExistente.ID;
                orderDB.OrderStatusDB = orderStatus;
                orderDB.CreatedAt = DateTime.Now;
                _context.Add(orderDB);
                _context.SaveChanges();
                return Ok("Order registrada com sucesso.");
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
                var order = _context.OrdersDBs.Where(o => o.ID == id).FirstOrDefault();
                if (order == null)
                    return BadRequest(JsonConvert.SerializeObject("Order não existe."));

                var orderStatus = _context.OrderStatusDBs.Where(o => o.StatusCode == orderStatusCodeDTO.StatusCode).FirstOrDefault();
                if (orderStatus == null)
                    return BadRequest(JsonConvert.SerializeObject("OrderStatus não existe."));

                order.OrderStatusDB = orderStatus;
                order.IdStatus = orderStatusCodeDTO.StatusCode;
                _context.Update(order);
                _context.SaveChanges();
                return Ok("Status alterado com sucesso.");
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
