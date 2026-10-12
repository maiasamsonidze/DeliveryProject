namespace DeliveryProject.Models
{
    public class SmsRequests
    {
        public record SendSmsRequest(int ParselCode, string Message);      
        public record SendRawSmsRequest(string PhoneNumber, string Message);
        public record SendBulkSmsRequest(List<string> PhoneNumbers, string Message);

        public record SendOrderSmsRequest(Guid ParselId, string? Message);

    }
}
