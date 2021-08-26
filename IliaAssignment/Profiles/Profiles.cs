using AutoMapper;
using IliaAssignment.Models.DB;
using IliaAssignment.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IliaAssignment.Profiles
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<CustomerDB, Customer>().ReverseMap();
            CreateMap<OrdersDB, Orders>().ReverseMap();
            CreateMap<OrderStatusDB, OrderStatus>().ReverseMap();
        }
    }
}
