using System;
using System.Collections.Generic;

namespace DeliveryProject.Models;

public partial class Refreshtoken
{
    public Guid Id { get; set; }

    public string Token { get; set; } = null!;

    public Guid Userid { get; set; }

    public DateTime Expiresat { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime? Revokedat { get; set; }

    public virtual User User { get; set; } = null!;
}
