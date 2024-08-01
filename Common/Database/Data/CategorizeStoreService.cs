using System;
using System.Collections.Generic;

namespace CRM.Common.Database.Data;

public partial class CategorizeStoreService
{
    public int Id { get; set; }

    public bool IsEnabled { get; set; }

    public int StoreServiceId { get; set; }

    public int ServiceCategoryId { get; set; }

    public int ServiceSubCategoryId { get; set; }

    public virtual ServiceCategory ServiceCategory { get; set; } = null!;

    public virtual ServiceSubCategory ServiceSubCategory { get; set; } = null!;

    public virtual StoreService StoreService { get; set; } = null!;
}
