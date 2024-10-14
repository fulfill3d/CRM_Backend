using Microsoft.EntityFrameworkCore;

namespace CRM.API.Business.Employee.Data.Database
{
    public partial class EmployeeContext(DbContextOptions<EmployeeContext> options) : DbContext(options)
    {
        
        public virtual DbSet<CRM.Common.Database.Data.Business> Businesses { get; set; }
        public virtual DbSet<CRM.Common.Database.Data.Store> Stores { get; set; }
        public virtual DbSet<CRM.Common.Database.Data.StoreEmployee> StoreEmployees { get; set; }

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