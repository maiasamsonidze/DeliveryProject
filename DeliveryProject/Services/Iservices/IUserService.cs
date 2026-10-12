using DeliveryProject.DTO;
using DeliveryProject.Models;
using DeliveryProject.Tokens;

namespace DeliveryProject.Services.Iservices
{
    public interface IUserService
    {
        Task <List <UserDto>> GetAllUsers();

        Task<string> UserRegistration(AddUserDto addUserDto);

        Task<BothTokenDto> UserLogin(string email, string password);

        Task<BothTokenDto> ModifyToken(RefreshTokenRequest refreshTokenRequest);

        Task RevokeAllActiveTokensAsync(Guid userId);


        Task<string> DeleteOwnAccount(Guid id, LoginUserDto loginUserDto);

        Task<string> UpdateOwnPassword(Guid id, ChangePasswordDto changePasswordDto);

        Task<List <Role>> GetAllRoles();

        Task<string> AddRoleByAdmin(AddRoleDto addRoleDto);






    }
}
