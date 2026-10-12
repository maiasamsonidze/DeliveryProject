using System;
using System.Collections.Generic;

namespace DeliveryProject.Models;

public partial class Parselstatus
{
    public Guid Id { get; set; }

    public string Statusname { get; set; } = null!;

    public string? Descriptionstatus { get; set; }

    public virtual ICollection<Userparsel> Userparsels { get; set; } = new List<Userparsel>();
}
