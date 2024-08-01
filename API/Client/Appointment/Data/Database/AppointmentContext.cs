using Microsoft.EntityFrameworkCore;

namespace CRM.API.Client.Appointment.Data.Database
{
    public partial class AppointmentContext(DbContextOptions<AppointmentContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            // //If the microservice has own context but separate db, add its config to the context
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

            // //If the microservice has own context and db, add its config to the context

            // base.OnModelCreating(modelBuilder);
            //
            // modelBuilder.Entity<Entity>(entity =>
            // {
            //     entity.HasKey(e => e.Id);
            //
            //     entity.Property(e => e.Id)
            //         .ValueGeneratedOnAdd();
            // });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public void RevertAllChangesInTheContext()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}