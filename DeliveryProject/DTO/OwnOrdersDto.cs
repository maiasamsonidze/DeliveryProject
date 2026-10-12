namespace DeliveryProject.DTO
{
    public class OwnOrdersDto
    {
        public int parselcode { get; set; }

        public string Pernumber { get; set; } = null!;

        public string Customername { get; set; } = null!;

        public string Customerphone { get; set; } = null!;

        public string Deliveryaddress { get; set; } = null!;

        public string Productname { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal Unitprice { get; set; }

        public string? Comments { get; set; }

        public decimal Productprice { get; set; }

        public decimal Deliveryprice { get; set; }

        public decimal Fullprice { get; set; }

    }
}
