using System;
using System.Collections.Generic;

namespace DeliveryProject.Models;

public partial class Parsel
{
    public Guid Id { get; set; }

    public Guid Orderid { get; set; }

    public int Parselcode { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<Userparsel> Userparsels { get; set; } = new List<Userparsel>();
}
