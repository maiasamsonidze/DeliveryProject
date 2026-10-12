using DeliveryProject.DTO;
using DeliveryProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryProject.Services.Iservices
{
    public interface IOrdersService
    {
        Task<List<OrderDto>> GetAllOrders();

        Task<List<OwnOrdersDto>> GetOwnOrders(Guid userId);



        Task<UploadOrderResult> UploadOrders(IFormFile? file, Guid userId);


        Task<string> DeleteOrderByAdmin(Guid orderId);

        Task<List<Parsel>> GetAllParsel();

        Task<string> AddParselCodeByAdmin(AddParselDto addParselDto);

        Task<string> UpdateParselCodeByAdmin(Guid parselId, int newCode);

        Task<string> DeleteParsel(Guid parselId);

        Task<string> UpdateOrderAddress(UpdateAddressDto updateAddressDto, Guid userId);







    }
}
