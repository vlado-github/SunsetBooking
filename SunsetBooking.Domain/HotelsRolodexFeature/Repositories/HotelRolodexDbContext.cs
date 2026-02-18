using Microsoft.EntityFrameworkCore;
using SunsetBooking.Domain.Base;
using SunsetBooking.Domain.Base.DbContext;
using SunsetBooking.Domain.HotelsRolodexFeature.Entities;
using SunsetBooking.Domain.Shared.Extensions;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Repositories;

public class HotelRolodexDbContext : CustomDbContext<HotelRolodexDbContext>
{
    public HotelRolodexDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public HotelRolodexDbContext(IUserContext userContext) : base(userContext)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public HotelRolodexDbContext(DbContextOptions<HotelRolodexDbContext> options, IUserContext userContext) 
        : base(options, userContext)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public virtual DbSet<Hotel> Hotels { get; set; }
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.SetDBOptions(
                connectionName: "HotelRolodexConnection", 
                connectionStringEnvVar:"SUNSET_BOOKING_HOTEL_ROLODEX_DB_CONNECTION", 
                migrationsHistorySchema: "public");
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Hotel Config

        var hotel = modelBuilder.Entity<Hotel>();
        hotel.ToTable("hotels", "hotel_rolodex");
        hotel.HasQueryFilter(x => !x.IsDeleted);
        hotel.HasKey(x => x.Id);
        hotel.Property(x => x.Location).HasColumnType("geography (point)");
        hotel.Property(x => x.Name).IsRequired().HasMaxLength(100);
        hotel.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);

        #endregion
    }
}