using AutoMapper;
using DeliveryService.Backend.Data.Entities;
using DeliveryService.Backend.DTOs;
using System.Net;

namespace DeliveryService.Backend.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Order, OrderDTO>();

            AllowNullCollections = true;
        }
    }
}
