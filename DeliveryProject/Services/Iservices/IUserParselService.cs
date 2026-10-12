using DeliveryProject.DTO;
using DeliveryProject.Models;

namespace DeliveryProject.Services.Iservices
{
    public interface IUserParselService
    {
        Task<List<UserParselDto>> GetAllUserParsel();

        Task<List<ParselMapDto>> GetOwnParsel(Guid userId);

        Task<string> AddUserParsel(AddUserParselDto addUserParselDto);

        Task<string> UpdateParselStatus(int parselCode, string newStatusName, Guid userId, bool isAdmin);

        Task<string> DeleteUserParsel(Guid userId, Guid parselId);

        Task<string> UpdateUserParsel(Guid userId, Guid parselId);

        Task<string> GetOrderPhoneForParselAsync(Guid parselId, Guid userId, bool isAdmin);

        Task<List<string>> GetPhoneNumbersForDeliverableParselsAsync(Guid userId);

        Task<string> GetOwnedParselPhoneAsync(int parselCode, Guid userId);

    }
}
