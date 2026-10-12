using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeliveryProject.SMSservices
{
    public class SmsOfficeSmsService:ISmsService
    {
        private readonly SmsOfficeOptions _options;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SmsOfficeSmsService> _logger;

        public SmsOfficeSmsService(IOptions<SmsOfficeOptions> options, HttpClient httpClient, ILogger<SmsOfficeSmsService> logger)
        {
            _options = options.Value;
            _httpClient = httpClient;
            _logger = logger;
        }


        public async Task<SmsResult> SendAsync(string toPhoneNumber, string messageBody, CancellationToken ct = default)
        {
            var normalizedNumber = NormalizeNumber(toPhoneNumber);
            var url = BuildUrl(normalizedNumber, messageBody);

            try
            {
                var response = await _httpClient.GetAsync(url, ct);
                var responseBody = await response.Content.ReadAsStringAsync(ct);

                var parsed = TryParseResponse(responseBody);

                if (parsed is null)
                {
                    _logger.LogWarning("smsoffice.ge პასუხის პარსინგი ვერ მოხერხდა {Phone}-ზე: {Body}",
                        normalizedNumber, responseBody);
                    return new SmsResult(normalizedNumber, false, null, "Unparseable response: " + responseBody);
                }

                if (!parsed.Success)
                {
                    _logger.LogWarning("smsoffice.ge SMS შეცდომა {Phone}-ზე: ErrorCode={Code}, Message={Message}",
                        normalizedNumber, parsed.ErrorCode, parsed.Message);
                    return new SmsResult(normalizedNumber, false, null, parsed.Message ?? $"ErrorCode {parsed.ErrorCode}");
                }

                return new SmsResult(normalizedNumber, true, parsed.ErrorCode.ToString(), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMS გაგზავნის შეცდომა {Phone}-ზე", normalizedNumber);
                return new SmsResult(normalizedNumber, false, null, ex.Message);
            }
        }


        public async Task<IReadOnlyList<SmsResult>> SendBulkAsync(
       IEnumerable<string> phoneNumbers, string messageBody, CancellationToken ct = default)
        {
            var tasks = phoneNumbers.Select(phone => SendAsync(phone, messageBody, ct));
            var results = await Task.WhenAll(tasks);
            return results;
        }

        private string BuildUrl(string destination, string content)
        {
            var encodedContent = Uri.EscapeDataString(content);
            var encodedSender = Uri.EscapeDataString(_options.Sender);

            return $"{_options.BaseUrl}?key={_options.ApiKey}" +
              $"&destination={destination}" +
              $"&sender={encodedSender}" +
              $"&content={encodedContent}";
        }
        private static string NormalizeNumber(string phoneNumber)
        {
            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (digitsOnly.StartsWith("00"))
                digitsOnly = digitsOnly[2..];

            if (!digitsOnly.StartsWith("995") && digitsOnly.Length == 9)
                digitsOnly = "995" + digitsOnly;

            return digitsOnly;
        }

        private SmsOfficeResponse? TryParseResponse(string responseBody)
        {
            try
            {
                return JsonSerializer.Deserialize<SmsOfficeResponse>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                return null;
            }
        }



        private class SmsOfficeResponse
        {
            [JsonPropertyName("Success")]
            public bool Success { get; set; }

            [JsonPropertyName("Message")]
            public string? Message { get; set; }

            [JsonPropertyName("Output")]
            public JsonElement? Output { get; set; }

            [JsonPropertyName("ErrorCode")]
            public int ErrorCode { get; set; }
        }


    }
}
