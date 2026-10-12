using DeliveryProject.DTO;
using DeliveryProject.Exceptions;
using DeliveryProject.Services;
using DeliveryProject.Services.Iservices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeliveryProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserParselController : ControllerBase
    {
        private readonly IUserParselService userParselService;

        public UserParselController(IUserParselService userParselService)
        {
            this.userParselService = userParselService;
        }

        [Authorize]
        [HttpPost("assign-parsel")]
        public async Task<ActionResult> AddUserParsel([FromBody] AddUserParselDto addUserParselDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Select(x => new
                {
                    Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                }).ToList());
            }

            try
            {
                var AdminUser = User.HasClaim(ClaimTypes.Role, "Admin");

                if (AdminUser)
                {
                    var result = await userParselService.AddUserParsel(addUserParselDto);
                    return Ok(result);


                }
                else
                {
                    return Unauthorized("მხოლოდ ადმინს შეუძლია მომხმარებლის სიის ნახვა");
                }


            }
            catch (UserException userException)
            {
                return BadRequest(userException.Message);
            }
            catch (OrderException orderException)
            {
                return BadRequest(orderException.Message);
            }
        }


        [Authorize]
        [HttpDelete("remove-userparsel")]
        public async Task<ActionResult> DeleteUserParsel([FromQuery] Guid userId, [FromQuery] Guid parselId)
        {
            try
            {
                var AdminUser = User.HasClaim(ClaimTypes.Role, "Admin");

                if (AdminUser)
                {
                    var result = await userParselService.DeleteUserParsel(userId, parselId);
                    return Ok(result);
                }
                else
                {
                    return Unauthorized("მხოლოდ ადმინს შეუძლია ამანათის მინიჭების წაშლა");
                }
            }
            catch (UserException userException)
            {
                return BadRequest(userException.Message);
            }
            catch (OrderException orderException)
            {
                return BadRequest(orderException.Message);
            }
        }



        [Authorize]
        [HttpGet]

        public async Task<ActionResult> GetAllUserParsel()
        {
            try
            {
                var AdminUser = User.HasClaim(ClaimTypes.Role, "Admin");
                if (AdminUser)
                {
                    var result = await userParselService.GetAllUserParsel();
                    return Ok(result);
                }
                else
                {
                    return Unauthorized("მხოლოდ ადმინს შეუძლია სიის ნახვა");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





        [Authorize]
        [HttpPut("update-parselstatus")]
        public async Task<ActionResult> UpdateParselStatus([FromBody] UpdateParselStatusDto updateParselStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Select(x => new
                {
                    Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                }).ToList());
            }

            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Unauthorized("მომხმარებელი არავალიდურია");

                var isAdmin = User.HasClaim(ClaimTypes.Role, "Admin");

                var result = await userParselService.UpdateParselStatus(
                    updateParselStatusDto.ParselCode, updateParselStatusDto.NewStatusName, userId, isAdmin);

                return Ok(result);
            }
            catch (UserException userException)
            {
                return BadRequest(userException.Message);
            }
            catch (OrderException orderException)
            {
                return BadRequest(orderException.Message);
            }
        }




    }
}
