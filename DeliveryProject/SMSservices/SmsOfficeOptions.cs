namespace DeliveryProject.SMSservices
{
    public class SmsOfficeOptions
    {
        public const string SectionName = "SmsOffice";

        public string ApiKey { get; set; } = string.Empty;
        public string Sender { get; set; } = "smsoffice";
        public string BaseUrl { get; set; } = "https://smsoffice.ge/api/v2/send/";

    }
}
