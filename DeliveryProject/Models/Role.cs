using System;
using System.Collections.Generic;

namespace DeliveryProject.Models;

public partial class Role
{
    public Guid Id { get; set; }

    public string Rolename { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
