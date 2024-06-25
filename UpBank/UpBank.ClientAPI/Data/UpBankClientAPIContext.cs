using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Bank;
using Models.People;

namespace UpBank.ClientAPI.Data
{
    public class UpBankClientAPIContext : DbContext
    {
        public UpBankClientAPIContext (DbContextOptions<UpBankClientAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Models.People.Client> Client { get; set; } = default!;
        
        public DbSet<Account> Accounts { get; set; } // Adicionei essa linha porque a entidade Account não estava sendo reconhecida
        public DbSet<Client> Clients { get; set; } // Aqui porque também não estava sendo reconhecida

    }
}
