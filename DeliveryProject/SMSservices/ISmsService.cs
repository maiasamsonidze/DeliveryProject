namespace DeliveryProject.SMSservices
{
    public interface ISmsService
    {
        Task<SmsResult> SendAsync(string toPhoneNumber, string messageBody, CancellationToken ct = default);
        Task<IReadOnlyList<SmsResult>> SendBulkAsync(IEnumerable<string> phoneNumbers, string messageBody, CancellationToken ct = default);
    }

    public record SmsResult(string PhoneNumber, bool Success, string? MessageId, string? ErrorMessage);
}


