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
            CreateMap<CustomerDB, CustomerDTO>().ReverseMap();
            CreateMap<CustomerDB, CustomerOrdersDTO>()
                .ForMember(co => co.OrdersDTO, cdb => cdb.MapFrom(customer => customer.OrdersDB))
                .ReverseMap();
            CreateMap<OrdersDB, OrdersDTO>()
                .ForMember(o => o.StatusName, odb => odb.MapFrom(order => order.OrderStatusDB.StatusName))
                .ReverseMap();
            CreateMap<OrdersDB, OrderDTO>()
                .ForMember(o => o.StatusCode, odb => odb.MapFrom(order => order.OrderStatusDB.StatusCode))
                .ReverseMap();
            CreateMap<OrderStatusDB, OrderStatusDTO>().ReverseMap();
        }
    }
}
