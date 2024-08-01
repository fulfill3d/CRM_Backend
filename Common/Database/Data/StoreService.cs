using System;
using System.Collections.Generic;

namespace CRM.Common.Database.Data;

public partial class StoreService
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsEnabled { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? Duration { get; set; }

    public decimal? Price { get; set; }

    public Guid BusinessRefId { get; set; }

    public int StoreId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<CategorizeStoreService> CategorizeStoreServices { get; set; } = new List<CategorizeStoreService>();

    public virtual Store Store { get; set; } = null!;
}
