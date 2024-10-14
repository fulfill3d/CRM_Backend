using Microsoft.EntityFrameworkCore;

namespace CRM.API.Business.Appointment.Data.Database
{
    public partial class AppointmentContext(DbContextOptions<AppointmentContext> options) : DbContext(options)
    {
        public virtual DbSet<CRM.Common.Database.Data.Store> Stores { get; set; }
        public virtual DbSet<CRM.Common.Database.Data.StoreService> StoreServices { get; set; }
        public virtual DbSet<CRM.Common.Database.Data.Appointment> Appointments { get; set; }

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