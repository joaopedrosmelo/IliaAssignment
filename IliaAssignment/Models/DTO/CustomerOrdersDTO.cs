using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IliaAssignment.Models.DTO
{
    public class CustomerOrdersDTO : CustomerDTO
    {
        public List<OrdersDTO> OrdersDTO { get; set; }
    }
}
