using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Bank;
using Models.People;

namespace UpBank.AgencyAPI.Data
{
    public class UpBankAgencyAPIContext : DbContext
    {
        public UpBankAgencyAPIContext (DbContextOptions<UpBankAgencyAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Bank.Agency> Agency { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<Address>();
            modelBuilder.Ignore<Employee>();
        }
    }
}
