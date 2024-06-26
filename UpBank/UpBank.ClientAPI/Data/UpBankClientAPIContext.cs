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

        public DbSet<Models.People.Client> Client { get; set; } = default!;

        // Adicionei encapsulamento abaixo, porque com o entity a entidade Account não estava sendo reconhecida,  precisei atualizar o seu contexto do Entity Framework
        public DbSet<Account> Accounts { get; set; } 
        public DbSet<ClientDTO> Clients { get; set; } 
        public DbSet<ClientCancelled> ClientCancelled { get; set; }
    }
}
