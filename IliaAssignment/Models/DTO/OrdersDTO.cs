using System;

namespace IliaAssignment.Models.DTO
{
    public class OrdersDTO
    {
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string StatusName { get; set; }
    }
}
