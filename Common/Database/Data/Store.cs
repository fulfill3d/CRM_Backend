using System;
using System.Collections.Generic;

namespace CRM.Common.Database.Data;

public partial class Store
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsEnabled { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid BusinessRefId { get; set; }

    public int BusinessId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Business Business { get; set; } = null!;

    public virtual ICollection<StoreEmployee> StoreEmployees { get; set; } = new List<StoreEmployee>();

    public virtual ICollection<StoreLocation> StoreLocations { get; set; } = new List<StoreLocation>();

    public virtual ICollection<StoreService> StoreServices { get; set; } = new List<StoreService>();
}
