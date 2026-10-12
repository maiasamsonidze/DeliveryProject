using AutoMapper;
using DeliveryProject.DTO;
using DeliveryProject.Models;

namespace DeliveryProject.Mapping
{
    public class RoleMapping : Profile
    {

        public RoleMapping()
        {
            CreateMap<AddRoleDto, Role>().ReverseMap();

        }
    }
}
