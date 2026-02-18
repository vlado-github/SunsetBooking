using NetTopologySuite.Geometries;
using SunsetBooking.Domain.Base;
using SunsetBooking.Domain.Base.Entity;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Entities;

public class Hotel : EntityBase, ISoftDeletable, IAuditable
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Point Location { get; set; }
    public bool IsDeleted { get; set; }
    public string CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ModifiedById { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
