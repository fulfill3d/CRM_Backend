using Microsoft.EntityFrameworkCore;

namespace CRM.Functions.Background.Data.Database
{
    public partial class BackgroundContext(DbContextOptions<BackgroundContext> options) : DbContext(options)
    {
        public virtual DbSet<CRM.Common.Database.Data.Appointment> Appointments { get; set; }
        public virtual DbSet<CRM.Common.Database.Data.AppointmentStatus> AppointmentStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CRM.Common.Database.Data.DatabaseContext).Assembly);

            OnModelCreatingPartial(modelBuilder);
        }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public void RevertAllChangesInTheContext()
        {
            ChangeTracker.Entries()
                .Where(e => e.Entity != null).ToList()
                .ForEach(e => e.State = EntityState.Detached);
        }
    }
}