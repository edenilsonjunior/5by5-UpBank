using Microsoft.EntityFrameworkCore;
using Models.Bank;
using Models.DTO;
using Models.People;

namespace UpBank.AgencyAPI.Data
{
    public class UpBankAgencyAPIContext : DbContext
    {
        public UpBankAgencyAPIContext(DbContextOptions<UpBankAgencyAPIContext> options)
            : base(options)
        {
        }

        public DbSet<AgencyDTOEntity> Agency { get; set; } = default!;
        public DbSet<EmployeeDTOEntity> Employee { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<Address>();

            modelBuilder.Entity<EmployeeDTOEntity>()
                .HasOne<AgencyDTOEntity>()
                .WithMany(a => a.Employees)
                .HasForeignKey(e => e.AgencyNumber)
                .HasPrincipalKey(a => a.Number);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=127.0.0.1; Initial Catalog=UpBank.AgencyAPI; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes");
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
