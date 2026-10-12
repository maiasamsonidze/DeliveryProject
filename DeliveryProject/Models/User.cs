using System;
using System.Collections.Generic;

namespace DeliveryProject.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Userpassword { get; set; } = null!;

    public string Phonenumber { get; set; } = null!;

    public Guid Roleid { get; set; }

    public DateTime Createdat { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Refreshtoken> Refreshtokens { get; set; } = new List<Refreshtoken>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Userparsel> Userparsels { get; set; } = new List<Userparsel>();
}
