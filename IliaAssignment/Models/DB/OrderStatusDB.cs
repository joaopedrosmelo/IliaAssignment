using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IliaAssignment.Models.DB
{
    [Table("OrderStatus")]
    public class OrderStatusDB
    {
        public int ID { get; set; }
        public int StatusCode { get; set; }
        public string StatusName { get; set; }
        public OrdersDB OrdersDB { get; set; }
    }
}
