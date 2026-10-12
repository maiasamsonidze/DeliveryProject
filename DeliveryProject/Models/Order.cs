using System;
using System.Collections.Generic;

namespace DeliveryProject.Models;

public partial class Order
{
    public Guid Id { get; set; }

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

    public Guid Userid { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public virtual ICollection<Parsel> Parsels { get; set; } = new List<Parsel>();

    public virtual User User { get; set; } = null!;
}
