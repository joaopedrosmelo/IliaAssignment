using AutoMapper;
using IliaAssignment.Data;
using IliaAssignment.Models.DB;
using IliaAssignment.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IliaAssignment.Controllers
{
    [Route("api/[controller]")]
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
            var customers = _mapper.Map<List<Customer>>(_context.CustomerDBs.ToList());
            return Ok(JsonConvert.SerializeObject(customers));
        }

        // GET api/<CustomersController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var customerOrder = _mapper.Map<CustomerOrder>(_context.CustomerDBs.Include(c => c.OrdersDB).ThenInclude(c => c.OrderStatusDB).Where(c => c.ID == id && c.OrdersDB != null).FirstOrDefault());
            return Ok(JsonConvert.SerializeObject(customerOrder));
        }

        // POST api/<CustomersController>
        [HttpPost]
        public ActionResult Post([FromBody] Customer customer)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(customer.Email);
            if (match.Success)
            {
                try
                {
                    var customerDB = _mapper.Map<CustomerDB>(customer);
                    _context.Add(customerDB);
                    _context.SaveChanges();
                    return Ok("Cliente cadastrado com sucesso.");
                }
                catch (System.Exception)
                {
                    return StatusCode(500);
                }
            }
            else
            {
                return BadRequest("E-mail inválido.");
            }
        }
    }
}
