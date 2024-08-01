using System;
using System.Collections.Generic;

namespace CRM.Common.Database.Data;

public partial class Business
{
    public int Id { get; set; }

    public Guid RefId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsEnabled { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}
