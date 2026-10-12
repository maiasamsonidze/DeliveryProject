using System;
using System.Collections.Generic;

namespace DeliveryProject.Models;

public partial class Userparsel
{
    public Guid Id { get; set; }

    public Guid Userid { get; set; }

    public Guid Parselid { get; set; }

    public Guid Statusid { get; set; }

    public DateTime Createdat { get; set; }

    public virtual Parsel Parsel { get; set; } = null!;

    public virtual Parselstatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
