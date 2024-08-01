using System;
using System.Collections.Generic;

namespace CRM.Common.Database.Data;

public partial class Appointment
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsEnabled { get; set; }

    public string? Notes { get; set; }

    public DateTime StartDate { get; set; }

    public int StatusId { get; set; }

    public int ClientId { get; set; }

    public int StoreServiceId { get; set; }

    public int StoreLocationId { get; set; }

    public int StoreEmployeeId { get; set; }

    public int StoreId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual AppointmentStatus Status { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;

    public virtual StoreEmployee StoreEmployee { get; set; } = null!;

    public virtual StoreLocation StoreLocation { get; set; } = null!;

    public virtual StoreService StoreService { get; set; } = null!;
}
