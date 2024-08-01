using System;
using System.Collections.Generic;

namespace CRM.Common.Database.Data;

public partial class ServiceSubCategory
{
    public int Id { get; set; }

    public bool IsEnabled { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<CategorizeStoreService> CategorizeStoreServices { get; set; } = new List<CategorizeStoreService>();
}
