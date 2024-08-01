using System;
using System.Collections.Generic;

namespace CRM.Common.Database.Data;

public partial class AppointmentStatus
{
    public int Id { get; set; }

    public bool IsEnabled { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
