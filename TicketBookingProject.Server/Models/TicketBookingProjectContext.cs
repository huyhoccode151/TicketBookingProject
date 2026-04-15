using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TicketBookingProject.Server.Models;

public partial class TicketBookingProjectContext : DbContext
{
    public TicketBookingProjectContext()
    {
    }

    public TicketBookingProjectContext(DbContextOptions<TicketBookingProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventPoster> EventPosters { get; set; }

    public virtual DbSet<EventSeat> EventSeats { get; set; }

    public virtual DbSet<EventSeatLog> EventSeatLogs { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<SeatHold> SeatHolds { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketType> TicketTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public DbSet<AuditLog> AuditLogs { get; set; }
    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    public virtual DbSet<VenueSection> VenueSections { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=TicketBookingProject;Trusted_Connection=True;TrustServerCertificate=True;Command Timeout=300");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("bookings");

            entity.HasIndex(e => e.CreatedAt, "IDX_b_created_at");

            entity.HasIndex(e => new { e.EventId, e.Status }, "IDX_b_event_status");

            entity.HasIndex(e => new { e.Status, e.ExpiresAt }, "IDX_b_status_expires");

            entity.HasIndex(e => e.UserId, "IDX_b_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CancelledReason)
                .HasMaxLength(255)
                .HasColumnName("cancelled_reason");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ExpiresAt)
                .HasPrecision(0)
                .HasColumnName("expires_at");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Event).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_b_event");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_b_user");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.ToTable("booking_details");

            entity.HasIndex(e => e.BookingId, "IDX_bd_booking_id");

            entity.HasIndex(e => e.EventSeatId, "IDX_bd_event_seat_id");

            entity.HasIndex(e => e.TicketTypeId, "IDX_bd_ticket_type_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.EventSeatId).HasColumnName("event_seat_id").IsRequired(false);
            entity.Property(e => e.TicketTypeId).HasColumnName("ticket_type_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity)
                .HasColumnName("quantity")
                .IsRequired()
                .HasDefaultValue(1);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_bd_booking");

            entity.HasOne(d => d.EventSeat).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.EventSeatId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_bd_event_seat");

            entity.HasOne(d => d.TicketType)
                .WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.TicketTypeId)
                .HasConstraintName("FK_bd_ticket_type");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");

            entity.HasIndex(e => e.Name, "UQ_categories_name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("events");

            entity.HasIndex(e => new { e.ActiveAt, e.EndAt }, "IDX_events_active_window");

            entity.HasIndex(e => e.CategoryId, "IDX_events_category_id");

            entity.HasIndex(e => e.OrganizerId, "IDX_events_organizer_id");

            entity.HasIndex(e => new { e.SaleStartAt, e.SaleEndAt }, "IDX_events_sale_window");

            entity.HasIndex(e => e.Status, "IDX_events_status");

            entity.HasIndex(e => new { e.Status, e.DeletedAt }, "IDX_events_status_deleted");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActiveAt)
                .HasPrecision(0)
                .HasColumnName("active_at");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasPrecision(0)
                .HasColumnName("deleted_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndAt)
                .HasPrecision(0)
                .HasColumnName("end_at");
            entity.Property(e => e.MaxTicketsPerBooking)
                .HasDefaultValue(10)
                .HasColumnName("max_tickets_per_booking");
            entity.Property(e => e.Name)
                .HasMaxLength(512)
                .HasColumnName("name");
            entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");
            entity.Property(e => e.SaleEndAt)
                .HasPrecision(0)
                .HasColumnName("sale_end_at");
            entity.Property(e => e.SaleStartAt)
                .HasPrecision(0)
                .HasColumnName("sale_start_at");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasColumnName("updated_at");
            entity.Property(e => e.VenueId).HasColumnName("venue_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Events)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ev_category");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Events)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ev_organizer");

            entity.HasOne(d => d.Venue).WithMany(p => p.Events)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ev_venue");
        });

        modelBuilder.Entity<EventPoster>(entity =>
        {
            entity.ToTable("event_posters");

            entity.HasIndex(e => e.EventId, "IDX_ep_event_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ImageType).HasColumnName("image_type");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(512)
                .HasColumnName("image_url");
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary");

            entity.HasOne(d => d.Event).WithMany(p => p.EventPosters)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK_ep_event");
        });

        modelBuilder.Entity<EventSeat>(entity =>
        {
            entity.ToTable("event_seats");

            entity.HasIndex(e => new { e.EventId, e.Status }, "IDX_es_event_status");

            entity.HasIndex(e => e.TicketTypeId, "IDX_es_ticket_type");

            entity.HasIndex(e => new { e.EventId, e.SeatId }, "UQ_es_event_seat").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.SeatId).HasColumnName("seat_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TicketTypeId).HasColumnName("ticket_type_id");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasColumnName("updated_at");
            entity.Property(e => e.Version).HasColumnName("version");

            entity.HasOne(d => d.Event).WithMany(p => p.EventSeats)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_es_event");

            entity.HasOne(d => d.Seat).WithMany(p => p.EventSeats)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_es_seat");

            entity.HasOne(d => d.TicketType).WithMany(p => p.EventSeats)
                .HasForeignKey(d => d.TicketTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_es_ticket_type");
        });

        modelBuilder.Entity<EventSeatLog>(entity =>
        {
            entity.ToTable("event_seat_logs");

            entity.HasIndex(e => e.BookingId, "IDX_esl_booking_id");

            entity.HasIndex(e => e.CreatedAt, "IDX_esl_created_at");

            entity.HasIndex(e => e.EventSeatId, "IDX_esl_event_seat_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.EventSeatId).HasColumnName("event_seat_id");
            entity.Property(e => e.NewStatus).HasColumnName("new_status");
            entity.Property(e => e.OldStatus).HasColumnName("old_status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.EventSeatLogs)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_esl_booking");

            entity.HasOne(d => d.EventSeat).WithMany(p => p.EventSeatLogs)
                .HasForeignKey(d => d.EventSeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_esl_event_seat");

            entity.HasOne(d => d.User).WithMany(p => p.EventSeatLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_esl_user");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");

            entity.HasIndex(e => new { e.BookingId, e.Status }, "IDX_pay_booking_status");

            entity.HasIndex(e => e.PaymentMethod, "IDX_pay_method");

            entity.HasIndex(e => e.Status, "IDX_pay_status");

            entity.HasIndex(e => e.PaymentTransaction, "UQ_payments_transaction").IsUnique();

            entity.HasIndex(e => new { e.UserId, e.IdempotencyKey }, "UQ_payments_user_idempotency").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(100)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.MetaData).HasColumnName("meta_data");
            entity.Property(e => e.PaymentMethod).HasColumnName("payment_method");
            entity.Property(e => e.PaymentTransaction)
                .HasMaxLength(255)
                .HasColumnName("payment_transaction");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pay_booking");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pay_user");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("permissions");

            entity.HasIndex(e => e.Action, "IDX_perm_action");

            entity.HasIndex(e => e.Resource, "IDX_perm_resource");

            entity.HasIndex(e => e.Name, "UQ_permissions_name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Resource)
                .HasMaxLength(50)
                .HasColumnName("resource");
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.ToTable("user_permissions");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Effect)
                .HasDefaultValue(1)
                .HasComment("1=allow, -1=deny (override role)");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2(0)");

            entity.HasIndex(e => new { e.UserId, e.PermissionId })
                .IsUnique();

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.Effect });

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Permission)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");

            entity.HasIndex(e => e.ExpiresAt, "IDX_rt_expires_at");

            entity.HasIndex(e => e.Status, "IDX_rt_status");

            entity.HasIndex(e => e.UserId, "IDX_rt_user_id");

            entity.HasIndex(e => new { e.UserId, e.Status }, "IDX_rt_user_status");

            entity.HasIndex(e => e.Token, "UQ_refresh_tokens_token").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.DeviceInfo)
                .HasMaxLength(255)
                .HasColumnName("device_info");
            entity.Property(e => e.ExpiresAt)
                .HasPrecision(0)
                .HasColumnName("expires_at");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.RevokedAt)
                .HasPrecision(0)
                .HasColumnName("revoked_at");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Token)
                .HasMaxLength(512)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_rt_user");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.ToTable("refunds");

            entity.HasIndex(e => e.BookingId, "IDX_refunds_booking_id");

            entity.HasIndex(e => e.PaymentId, "IDX_refunds_payment_id");

            entity.HasIndex(e => e.Status, "IDX_refunds_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.ProcessedAt)
                .HasPrecision(0)
                .HasColumnName("processed_at");
            entity.Property(e => e.ProcessedBy).HasColumnName("processed_by");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .HasColumnName("reason");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Booking).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ref_booking");

            entity.HasOne(d => d.Payment).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ref_payment");

            entity.HasOne(d => d.ProcessedByNavigation).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.ProcessedBy)
                .HasConstraintName("FK_ref_processed_by");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "UQ_roles_name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .HasConstraintName("FK_rp_permission"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_rp_role"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId");
                        j.ToTable("role_permissions");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.ToTable("seats");

            entity.HasIndex(e => e.SectionId, "IDX_seats_section_id");

            entity.HasIndex(e => new { e.VenueId, e.Row, e.SeatNumber }, "IDX_seat_position");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.Row)
                .HasMaxLength(10)
                .HasColumnName("row");
            entity.Property(e => e.SeatNumber)
                .HasMaxLength(10)
                .HasColumnName("seat_number");
            entity.Property(e => e.SeatType).HasColumnName("seat_type");
            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.VenueId).HasColumnName("venue_id");

            entity.HasOne(d => d.Section).WithMany(p => p.Seats)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_seat_section");

            entity.HasOne(d => d.Venue).WithMany(p => p.Seats)
                .HasForeignKey(d => d.VenueId)
                .HasConstraintName("FK_seat_venue");
        });

        modelBuilder.Entity<SeatHold>(entity =>
        {
            entity.ToTable("seat_holds");

            entity.HasIndex(e => e.ExpiresAt, "IDX_sh_expires_at");

            entity.HasIndex(e => new { e.Status, e.ExpiresAt }, "IDX_sh_status_exp");

            entity.HasIndex(e => new { e.UserId, e.Status }, "IDX_sh_user_status");

            entity.HasIndex(e => e.EventSeatId, "UQ_sh_event_seat_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.EventSeatId).HasColumnName("event_seat_id");
            entity.Property(e => e.TicketTypeId).HasColumnName("ticket_type_id");
            entity.Property(e => e.Quantity).IsRequired().HasColumnName("quantity");
            entity.Property(e => e.ExpiresAt)
                .HasPrecision(0)
                .HasColumnName("expires_at");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(e => e.TicketType)
                .WithMany(t => t.SeatHolds)
                .HasForeignKey(e => e.TicketTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Booking).WithMany(p => p.SeatHolds)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sh_booking");

            entity.HasOne(d => d.EventSeat).WithOne(p => p.SeatHold)
                .HasForeignKey<SeatHold>(d => d.EventSeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sh_event_seat");

            entity.HasOne(d => d.User).WithMany(p => p.SeatHolds)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sh_user");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("tickets");

            entity.HasIndex(e => e.BookingId, "IDX_tickets_booking_id");

            entity.HasIndex(e => e.EventSeatId, "IDX_tickets_event_seat_id");

            entity.HasIndex(e => e.Status, "IDX_tickets_status");

            entity.HasIndex(e => e.TicketTypeId, "IDX_tickets_ticket_type_id");

            entity.HasIndex(e => e.QrCode, "UQ_tickets_qr_code").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CheckedInAt)
                .HasPrecision(0)
                .HasColumnName("checked_in_at");
            entity.Property(e => e.CheckedInBy).HasColumnName("checked_in_by");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.EventSeatId).HasColumnName("event_seat_id");
            entity.Property(e => e.TicketTypeId).HasColumnName("ticket_type_id");
            entity.Property(e => e.QrCode)
                .HasMaxLength(255)
                .HasColumnName("qr_code");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_t_booking");

            entity.HasOne(d => d.CheckedInByNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CheckedInBy)
                .HasConstraintName("FK_t_checked_by");

            entity.HasOne(d => d.EventSeat).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.EventSeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_t_event_seat");

            entity.HasOne(t => t.TicketType)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.TicketTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_t_ticket_type");
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.ToTable("ticket_types");

            entity.HasIndex(e => e.EventId, "IDX_tt_event_id");

            entity.HasIndex(e => e.Status, "IDX_tt_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.MaxPerUser).HasColumnName("max_per_user");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SoldQuantity).HasColumnName("sold_quantity");
            entity.Property(e => e.Status)
                .HasDefaultValue(TicketTypeStatus.OnSale)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasColumnName("updated_at");
            entity.Property(e => e.Version).HasColumnName("version");

            entity.HasOne(d => d.Event).WithMany(p => p.TicketTypes)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK_tt_event");
        });


        modelBuilder.Entity<User>().HasQueryFilter(u => u.DeletedAt == null);
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasIndex(e => e.DeletedAt, "IDX_users_deleted_at");

            entity.HasIndex(e => e.Status, "IDX_users_status");

            entity.HasIndex(e => e.Email, "UQ_users_email").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_users_username").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasPrecision(0)
                .HasColumnName("deleted_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EmailVerifiedAt)
                .HasPrecision(0)
                .HasColumnName("email_verified_at");
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.LoginType)
                .HasDefaultValue(LoginType.Email)
                .HasColumnName("login_type");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.AccessFailedCount).HasColumnName("access_failed_count");
            entity.Property(e => e.LockoutEnd)
                .HasPrecision(0)
                .HasColumnName("lockout_end");
            entity.Property(e => e.Status)
                .HasDefaultValue(UserStatus.Active)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_ur_role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_ur_user"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("user_roles");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                    });
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.ToTable("venues");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddressDetail)
                .HasMaxLength(1024)
                .HasColumnName("address_detail");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(10, 7)")
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(10, 7)")
                .HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Province)
                .HasMaxLength(255)
                .HasColumnName("province");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<VenueSection>(entity =>
        {
            entity.ToTable("venue_sections");

            entity.HasIndex(e => e.VenueId, "IDX_vs_venue_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.VenueId).HasColumnName("venue_id");

            entity.HasOne(d => d.Venue).WithMany(p => p.VenueSections)
                .HasForeignKey(d => d.VenueId)
                .HasConstraintName("FK_vs_venue");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.EntityType)
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(x => x.Metadata)
                .HasColumnType("nvarchar(max)");

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.Action);
            entity.HasIndex(x => x.CreatedAt);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
