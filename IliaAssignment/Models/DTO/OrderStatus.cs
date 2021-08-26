using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IliaAssignment.Models.DTO
{
    public class OrderStatus
    {
        public int StatusCode { get; set; }
        public string StatusName { get; set; }
    }
}
