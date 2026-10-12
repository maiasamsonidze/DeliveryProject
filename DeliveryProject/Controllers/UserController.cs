using DeliveryProject.DTO;
using DeliveryProject.Exceptions;
using DeliveryProject.Services.Iservices;
using DeliveryProject.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeliveryProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [Authorize]
        [HttpGet]

        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var AdminUser = User.HasClaim(ClaimTypes.Role, "Admin");

                if (AdminUser)
                {
                    var result = await userService.GetAllUsers();
                    return Ok(result);


                }
                else
                {
                    return Unauthorized("მხოლოდ ადმინს შეუძლია მომხმარებლის სიის ნახვა");
                }

            }
            catch (Exception CustomerException)
            {

                return BadRequest(CustomerException.Message);
            }


        }



        [HttpPost("Authorization")]

        public async Task<ActionResult> UserLogin([FromBody] LoginUserDto loginUserDto)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Select(x => new
                {

                    Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                }).ToList());
            }
            else
            {
                try
                {
                    var result = await userService.UserLogin(loginUserDto.Email, loginUserDto.Password);

                    return Ok(result);
                }
                catch (EmailException emailExeption)
                {

                    return NotFound(emailExeption.Message);
                }
                catch (PasswordException passwordExeption)
                {

                    return BadRequest(passwordExeption.Message);

                }
            }


        }




        [HttpPost("Registration")]

        public async Task<ActionResult> AddUser(AddUserDto addUserDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Select(x => new
                {

                    Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                }).ToList());
            }
            else
            {
                try
                {
                    var result = await userService.UserRegistration(addUserDto);
                    return Ok(result);
                }
                catch (UserException userException)
                {

                    return BadRequest(userException.Message);
                }

            }

        }


        [HttpPost("ModifyToken")]

        public async Task<ActionResult> ModifyRefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                var result = await userService.ModifyToken(refreshTokenRequest);

                return Ok(result);
            }
            catch (TokenException exception)
            {

                return BadRequest(exception.Message);
            }
            catch (RefreshTokenException refreshTokenException)
            {
                return BadRequest(refreshTokenException.Message);
            }

            catch (UserException userexception)
            {
                return NotFound(userexception.Message);
            }
        }



        [Authorize]
        [HttpDelete]

        public async Task<ActionResult> DeleteUser(LoginUserDto loginUserDto)
        {
            try
            {

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized("მომხმარებელი არავალიდურია");
                }

                var result = await userService.DeleteOwnAccount(userId, loginUserDto);

                return Ok(result);

            }
            catch (UserException userexception)
            {

                return NotFound(userexception.Message);
            }
            catch (EmailException emailexception)
            {
                return BadRequest(emailexception.Message);
            }
            catch (PasswordException passwordexception)
            {
                return BadRequest(passwordexception.Message);
            }

        }

        [Authorize]
        [HttpPut("UpdateOwnPassword")]

        public async Task<ActionResult> UpdateUserPassword(ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Select(x => new
                {

                    Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                }).ToList());
            }
            else
            {


                try
                {
                    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out Guid userId))
                    {
                        return Unauthorized("მომხმარებელი არავალიდურია");
                    }

                    var result = await userService.UpdateOwnPassword(userId, changePasswordDto);

                    return Ok(result);
                }
                catch (OldPasswordException oldpasswordexception)
                {

                    return BadRequest(oldpasswordexception.Message);
                }
                catch (SamePasswordException samepasswordexception)
                {
                    return BadRequest(samepasswordexception.Message);
                }
                catch (UserException userexception)
                {
                    return NotFound(userexception.Message);
                }

            }

        }



        [Authorize]

        [HttpGet("GetAllRoles")]

        public async Task<ActionResult> GetAllRoles()
        {
            try
            {
                var AdminUser = User.HasClaim(ClaimTypes.Role, "Admin");
                if (AdminUser)
                {
                    var result = await userService.GetAllRoles();
                    return Ok(result);
                }
                else
                {
                    return Unauthorized("მხოლოდ ადმინს შეუძლია როლების სიის ნახვა");
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }


        [Authorize]
        [HttpPost("AddRoleByAdmin")]


        public async Task<ActionResult> AddRoleByAdmin(AddRoleDto addRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Select(x => new
                {

                    Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                }).ToList());
            }
            else
            {

                try
                {
                    var AdminUser = User.HasClaim(ClaimTypes.Role, "Admin");
                    if (AdminUser)
                    {
                        var result = await userService.AddRoleByAdmin(addRoleDto);
                        return Ok(result);
                    }
                    else
                    {
                        return Unauthorized("მხოლოდ ადმინს შეუძლია როლის დამატება");
                    }
                }
                catch (Exception exception)
                {
                    return BadRequest(exception.Message);
                }


            }


        }












    }

}
