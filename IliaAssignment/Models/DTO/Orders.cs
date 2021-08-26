using System;

namespace IliaAssignment.Models.DTO
{
    public class Orders
    {
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
