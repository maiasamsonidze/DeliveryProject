namespace DeliveryProject.Models
{
    public class UploadOrderResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new();

    }
}
