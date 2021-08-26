using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IliaAssignment.Models.DTO
{
    public class Customer
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Orders> Orders { get; set; }
    }
}
