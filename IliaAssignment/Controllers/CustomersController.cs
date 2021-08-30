using AutoMapper;
using IliaAssignment.Data;
using IliaAssignment.Models.DB;
using IliaAssignment.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IliaAssignment.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private static IMapper _mapper;
        public CustomersController(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // GET: api/<CustomersController>
        
        [HttpGet]
        public ActionResult Get()
        {
            var customers = _mapper.Map<List<CustomerDTO>>(_context.CustomerDBs.ToList());
            return Ok(JsonConvert.SerializeObject(customers));
        }

        // GET api/<CustomersController>/5
        
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var customerOrder = _mapper.Map<CustomerOrdersDTO>(_context.CustomerDBs.Include(c => c.OrdersDB).ThenInclude(c => c.OrderStatusDB).Where(c => c.ID == id && c.OrdersDB != null).FirstOrDefault());
            return Ok(JsonConvert.SerializeObject(customerOrder));
        }

        // POST api/<CustomersController>
        
        [HttpPost]
        public ActionResult Post([FromBody] CustomerDTO customerDTO)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$");
            Match match = regex.Match(customerDTO.Email);
            if (match.Success && !String.IsNullOrEmpty(customerDTO.Name))
            {
                try
                {
                    var customer = _context.CustomerDBs.Where(c => c.Email == customerDTO.Email).FirstOrDefault();
                    if (customer != null)
                        return BadRequest(JsonConvert.SerializeObject("Customer já cadastrado."));

                    var customerDB = _mapper.Map<CustomerDB>(customerDTO);
                    _context.Add(customerDB);
                    _context.SaveChanges();
                    return Ok("Cliente cadastrado com sucesso.");
                }
                catch
                {
                    return StatusCode(500);
                }
            }
            else
            {
                List<string> errorMessage = new List<string>();

                if (String.IsNullOrEmpty(customerDTO.Name))
                    errorMessage.Add("O nome deve ser preenchido.");
                if (!match.Success)
                    errorMessage.Add("E-mail inválido.");

                return BadRequest(JsonConvert.SerializeObject(errorMessage));
            }
        }
    }
}
