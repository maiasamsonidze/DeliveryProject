namespace DeliveryProject.DTO
{
    public class ParselMapDto
    {
        public Guid OrderId { get; set; }

        public string Address { get; set; } = null!;

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string status { get; set; } 

    }
}
