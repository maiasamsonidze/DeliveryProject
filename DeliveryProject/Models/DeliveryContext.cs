using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DeliveryProject.Models;

public partial class DeliveryContext : DbContext
{
    public DeliveryContext()
    {
    }

    public DeliveryContext(DbContextOptions<DeliveryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Parsel> Parsels { get; set; }

    public virtual DbSet<Parselstatus> Parselstatuses { get; set; }

    public virtual DbSet<Refreshtoken> Refreshtokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userparsel> Userparsels { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-6CLB7NT\\SQLEXPRESS; Database=DELIVERY;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ORDERS__3214EC27B539A87F");

            entity.ToTable("ORDERS");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Comments)
                .HasMaxLength(500)
                .HasColumnName("COMMENTS");
            entity.Property(e => e.Customername)
                .HasMaxLength(100)
                .HasColumnName("CUSTOMERNAME");
            entity.Property(e => e.Customerphone)
                .HasMaxLength(20)
                .HasColumnName("CUSTOMERPHONE");
            entity.Property(e => e.Deliveryaddress)
                .HasMaxLength(300)
                .HasColumnName("DELIVERYADDRESS");
            entity.Property(e => e.Deliveryprice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DELIVERYPRICE");
            entity.Property(e => e.Fullprice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("FULLPRICE");
            entity.Property(e => e.Pernumber)
                .HasMaxLength(20)
                .HasColumnName("PERNUMBER");
            entity.Property(e => e.Productname)
                .HasMaxLength(150)
                .HasColumnName("PRODUCTNAME");
            entity.Property(e => e.Productprice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PRODUCTPRICE");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");
            entity.Property(e => e.Unitprice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("UNITPRICE");
            entity.Property(e => e.Userid).HasColumnName("USERID");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("FK__ORDERS__USERID__5FB337D6");
        });

        modelBuilder.Entity<Parsel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PARSEL__3214EC27D5577A60");

            entity.ToTable("PARSEL");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Orderid).HasColumnName("ORDERID");
            entity.Property(e => e.Parselcode).HasColumnName("PARSELCODE");

            entity.HasOne(d => d.Order).WithMany(p => p.Parsels)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("FK__PARSEL__ORDERID__6754599E");
        });

        modelBuilder.Entity<Parselstatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PARSELST__3214EC27F956280E");

            entity.ToTable("PARSELSTATUS");

            entity.HasIndex(e => e.Statusname, "UQ__PARSELST__3144C027649FFD36").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Descriptionstatus)
                .HasMaxLength(255)
                .HasColumnName("DESCRIPTIONSTATUS");
            entity.Property(e => e.Statusname)
                .HasMaxLength(100)
                .HasColumnName("STATUSNAME");
        });

        modelBuilder.Entity<Refreshtoken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__REFRESHT__3214EC2713C3851D");

            entity.ToTable("REFRESHTOKEN");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATEDAT");
            entity.Property(e => e.Expiresat)
                .HasColumnType("datetime")
                .HasColumnName("EXPIRESAT");
            entity.Property(e => e.Revokedat)
                .HasColumnType("datetime")
                .HasColumnName("REVOKEDAT");
            entity.Property(e => e.Token).HasColumnName("TOKEN");
            entity.Property(e => e.Userid).HasColumnName("USERID");

            entity.HasOne(d => d.User).WithMany(p => p.Refreshtokens)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__REFRESHTO__USERI__04E4BC85");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ROLES__3214EC276334BEE8");

            entity.ToTable("ROLES");

            entity.HasIndex(e => e.Rolename, "UQ__ROLES__7E681897BD7E9869").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Rolename)
                .HasMaxLength(100)
                .HasColumnName("ROLENAME");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USERS__3214EC275BBB4051");

            entity.ToTable("USERS");

            entity.HasIndex(e => e.Email, "UQ__USERS__161CF72483B6BDEC").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("CREATEDAT");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PHONENUMBER");
            entity.Property(e => e.Roleid).HasColumnName("ROLEID");
            entity.Property(e => e.Username)
                .HasMaxLength(200)
                .HasColumnName("USERNAME");
            entity.Property(e => e.Userpassword)
                .HasMaxLength(100)
                .HasColumnName("USERPASSWORD");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("FK__USERS__ROLEID__5BE2A6F2");
        });

        modelBuilder.Entity<Userparsel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USERPARS__3214EC27477B5B24");

            entity.ToTable("USERPARSEL");

            entity.HasIndex(e => new { e.Userid, e.Parselid }, "UQ_UserParsel").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("CREATEDAT");
            entity.Property(e => e.Parselid).HasColumnName("PARSELID");
            entity.Property(e => e.Statusid).HasColumnName("STATUSID");
            entity.Property(e => e.Userid).HasColumnName("USERID");

            entity.HasOne(d => d.Parsel).WithMany(p => p.Userparsels)
                .HasForeignKey(d => d.Parselid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USERPARSE__PARSE__72C60C4A");

            entity.HasOne(d => d.Status).WithMany(p => p.Userparsels)
                .HasForeignKey(d => d.Statusid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USERPARSE__STATU__73BA3083");

            entity.HasOne(d => d.User).WithMany(p => p.Userparsels)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USERPARSE__USERI__71D1E811");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
