using DeliveryProject.Exceptions;
using DeliveryProject.Models;
using DeliveryProject.Services;
using DeliveryProject.Services.Iservices;
using DeliveryProject.SMSservices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static DeliveryProject.Models.SmsRequests;

namespace DeliveryProject.Controllers
{
    [Authorize(Roles = "DeliveryAgent,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ISmsService _smsService;
        private readonly IUserParselService _userParselService;

        public NotificationController(ISmsService smsService, IUserParselService userParselService)
        {
            _smsService = smsService;
            _userParselService = userParselService;
        }

        [HttpPost("sms/order-failed-delivery")]
        public async Task<IActionResult> SendOrderFailedDeliverySms( [FromBody] SendOrderSmsRequest request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("მომხმარებელი არავალიდურია");

            var isAdmin = User.HasClaim(ClaimTypes.Role, "Admin");

            string phone;
            try
            {
                if (request == null)
                {
                    return BadRequest("აიდის ფორმატი არასწორია");
                }

               
                phone = await _userParselService.GetOrderPhoneForParselAsync(request.ParselId, userId, isAdmin);
            }
            catch (OrderException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UserException ex)
            {
                return StatusCode(403, ex.Message);
            }

            var message = string.IsNullOrWhiteSpace(request.Message)
                ? "თქვენი შეკვეთის ჩაბარება ვერ მოხერხდა. დაგვიკავშირდით დამატებითი ინფორმაციისთვის."
                : request.Message;

            var result = await _smsService.SendAsync(phone, message, ct);
            return result.Success ? Ok(result) : StatusCode(502, result);
        }


        [HttpPost("sms/single")]
        public async Task<IActionResult> SendSingle([FromBody] SendSmsRequest request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("მომხმარებელი არავალიდურია");
            }

            if (request.ParselCode <= 0 || string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("ამანათის კოდი და ტექსტი სავალდებულოა");

            string phone;
            try
            {
                phone = await _userParselService.GetOwnedParselPhoneAsync(request.ParselCode, userId);
            }
            catch (OrderException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UserException ex)
            {
                return StatusCode(403, ex.Message);
            }

            var result = await _smsService.SendAsync(phone, request.Message, ct);
            return result.Success ? Ok(result) : StatusCode(502, result);
        }


        [Authorize]
        [HttpPost("sms/raw")]
        public async Task<IActionResult> SendRaw(
            [FromBody] SendRawSmsRequest request,
            CancellationToken ct)
        {
            if (!User.IsInRole("Admin"))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "მხოლოდ Admin-ს შეუძლია ამ ოპერაციის შესრულება."
                );
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("ნომერი და ტექსტი სავალდებულოა");
            }

            var result = await _smsService.SendAsync(
                request.PhoneNumber,
                request.Message,
                ct);

            return result.Success? Ok(result): StatusCode(502, result);
        }



        [HttpPost("sms/bulk")]
        public async Task<IActionResult> SendBulk([FromBody] SendBulkSmsRequest request, CancellationToken ct)
        {
            if (request.PhoneNumbers is null || request.PhoneNumbers.Count == 0)
                return BadRequest("სულ მცირე ერთი ნომერია საჭირო");

            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("ტექსტი სავალდებულოა");

            var cleanedNumbers = request.PhoneNumbers
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct()
                .ToList();

            if (cleanedNumbers.Count == 0)
                return BadRequest("სულ მცირე ერთი ვალიდური ნომერია საჭირო");

            if (cleanedNumbers.Count > 500)
                return BadRequest("ერთდროულად მაქსიმუმ 500 ნომერია დაშვებული");

            var results = await _smsService.SendBulkAsync(cleanedNumbers, request.Message, ct);

            return Ok(new
            {
                total = results.Count,
                sent = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }



        [HttpPost("sms/order-bulk-notify")]
        public async Task<IActionResult> SendBulkDeliveryNotification(CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("მომხმარებელი არავალიდურია");

            List<string> phones;
            try
            {
                phones = await _userParselService.GetPhoneNumbersForDeliverableParselsAsync(userId);
            }
            catch (OrderException ex)
            {
                return NotFound(ex.Message);
            }

            const string message =
                "თქვენი ამანათი გადაეცა კურიერს. კურიერი ამანათს ჩაგაბარებთ 3 სამუშაო დღის განმავლობაში";

            var results = await _smsService.SendBulkAsync(phones, message, ct);

            return Ok(new
            {
                total = results.Count,
                sent = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                details = results
            });
        }





    }
}
