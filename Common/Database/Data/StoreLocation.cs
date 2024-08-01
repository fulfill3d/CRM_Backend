using NetTopologySuite.Geometries;

namespace CRM.Common.Database.Data;

public partial class StoreLocation
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsEnabled { get; set; }

    public string Name { get; set; } = null!;

    public int StoreId { get; set; }

    public int AddressId { get; set; }
    
    public Geometry Location { get; set; } = null!;

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Store Store { get; set; } = null!;
}
