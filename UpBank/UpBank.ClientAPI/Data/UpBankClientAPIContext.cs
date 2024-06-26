using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Bank;
using Models.DTO;
using Models.People;

namespace UpBank.ClientAPI.Data
{
    public class UpBankClientAPIContext : DbContext
    {
        public UpBankClientAPIContext (DbContextOptions<UpBankClientAPIContext> options)
            : base(options)
        {
        }

        public DbSet<ClientDTO> Clients { get; set; } 
        public DbSet<ClientCancelled> DeletedClient { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=127.0.0.1; Initial Catalog=UpBankClientAPI; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes");
        }

    }
}
