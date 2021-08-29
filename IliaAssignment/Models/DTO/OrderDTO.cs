using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IliaAssignment.Models.DTO
{
    public class OrderDTO
    {
        public decimal Price { get; set; }
        public int StatusCode { get; set; }
        public string CustomerEmail { get; set; }
    }
}
