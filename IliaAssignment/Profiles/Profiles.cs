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
            CreateMap<CustomerDB, CustomerOrder>()
                .ForMember(co => co.Orders, cdb => cdb.MapFrom(customer => customer.OrdersDB))
                .ReverseMap();
            CreateMap<OrdersDB, Orders>()
                .ForMember(o => o.Status, odb => odb.MapFrom(order => order.OrderStatusDB.StatusName))
                .ReverseMap();
            CreateMap<OrderStatusDB, OrderStatus>().ReverseMap();
        }
    }
}
