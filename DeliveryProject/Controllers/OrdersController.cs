using DeliveryProject.DTO;
using DeliveryProject.Exceptions;
using DeliveryProject.Services;
using DeliveryProject.Services.Iservices;
using DocumentFormat.OpenXml.Office2019.Drawing.Model3D;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeliveryProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        private readonly IOrdersService ordersService;
        private readonly IUserParselService courierService;


        public OrdersController(IOrdersService ordersService, IUserParselService courierService)
        {
            this.ordersService = ordersService;
            this.courierService = courierService;
        }


        [Authorize]
        [HttpPost("upload-orders")]
        public async Task<ActionResult> UploadOrders(IFormFile? file)
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

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("მომხმარებელი ვერ მოიძებნა.");
                }

                Guid userId = Guid.Parse(userIdClaim);


                var result = await ordersService.UploadOrders(file, userId);

                if (!result.Success)
                {
                    return BadRequest(result.Errors);
                }

                return Ok("შეკვეთები წარმატებით აიტვირთა.");
            }

        }


        [Authorize]
        [HttpGet("get-all-orders")]

        public async Task<ActionResult> GetAllOrders()
        {
            try
            {
                var AdminUser = User.HasClaim(ClaimTypes.Role, "Admin");
                if (AdminUser)
                {
                    var result = await ordersService.GetAllOrders();
                    return Ok(result);
                }
                else
                {
                    return Unauthorized("მხოლოდ ადმინს შეუძლია როლების სიის ნახვა");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize]
        [HttpGet("get-own-orders")]

        public async Task<ActionResult> GetOwnOrders()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("მომხმარებელი ვერ მოიძებნა.");
                }
                Guid userId = Guid.Parse(userIdClaim);
                var result = await ordersService.GetOwnOrders(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpPost("add-parsel-code")]

        public async Task<ActionResult> AddParselCodeByAdmin([FromBody] AddParselDto addParselDto)
        {
            if(!ModelState .IsValid)
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
                        var result = await ordersService.AddParselCodeByAdmin(addParselDto);
                        return Ok(result);
                    }
                    else
                    {
                        return Unauthorized("მხოლოდ ადმინს შეუძლია ამანათის კოდის დამატება");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }


            }
               

        }


        [Authorize]

        [HttpGet("get-all-parsel")]


        public async Task<ActionResult> GetAllParsel()
        {
            try
            {
                var AdminUser = User.HasClaim(ClaimTypes.Role, "Admin");
                if (AdminUser)
                {
                    var result = await ordersService.GetAllParsel();
                    return Ok(result);
                }
                else
                {
                    return Unauthorized("მხოლოდ ადმინს შეუძლია ამანათების სიის ნახვა");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize]
        [HttpDelete("delete-parsel/{parselId}")]

        public async Task<ActionResult> DeleteParsel(Guid parselId)
        {
            try
            {
                var AdminUser = User.HasClaim(ClaimTypes.Role, "Admin");
                if (AdminUser)
                {
                    var result = await ordersService.DeleteParsel(parselId);
                    return Ok(result);
                }
                else
                {
                    return Unauthorized("მხოლოდ ადმინს შეუძლია ამანათის წაშლა");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



        [Authorize]
        [HttpDelete("delete-order/{orderId}")]

        public async Task<ActionResult> DeleteOrderByAdmin(Guid orderId)
        {
            try
            {
                var AdminUser = User.HasClaim(ClaimTypes.Role, "Admin");
                if (AdminUser)
                {
                    var result = await ordersService.DeleteOrderByAdmin(orderId);
                    return Ok(result);
                }
                else
                {
                    return Unauthorized("მხოლოდ ადმინს შეუძლია შეკვეთის წაშლა");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [Authorize]
        [HttpPut("update-parsel/{parselId}")]

        public async Task<ActionResult> UpdateParselCodeByAdmin(Guid parselId, [FromBody] int newCode)
        {
            try
            {
                var AdminUser = User.HasClaim(ClaimTypes.Role, "Admin");
                if (AdminUser)
                {
                    var result = await ordersService.UpdateParselCodeByAdmin(parselId, newCode);
                    return Ok(result);
                }
                else
                {
                    return Unauthorized("მხოლოდ ადმინს შეუძლია ამანათის კოდის განახლება");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }




        [Authorize]
        [HttpPut("update-address")]
        public async Task<ActionResult> UpdateAddress([FromBody] UpdateAddressDto updateAddressDto)
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

                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized("მომხმარებელი არავალიდურია");
                }
                var result = await ordersService.UpdateOrderAddress(updateAddressDto, userId);
                    return Ok(result);
                
            }
            catch (UserException userecxeption)
            {
                return BadRequest(userecxeption.Message);
            }

            catch (TokenException tokenexception)
            {
                return BadRequest(tokenexception.Message);
            }
            catch (AddressException addressexception)
            {
                return BadRequest(addressexception.Message);
            }
        }



        [Authorize]
        [HttpGet("my-parcels-map")]
        public async Task<IActionResult> GetMyParcelsMap()
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var parcels = await courierService.GetOwnParsel(userId);

            return Ok(parcels);
        }






    }

}
