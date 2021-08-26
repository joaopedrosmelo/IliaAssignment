using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IliaAssignment.Models.DB
{
    [Table("Orders")]
    public class OrdersDB
    {
        public int ID { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public int IdCustomer { get; set; }
        public int IdStatus { get; set; }
        public CustomerDB CustomerDB { get; set; }
        public OrderStatusDB OrderStatusDB { get; set; }
    }
}
