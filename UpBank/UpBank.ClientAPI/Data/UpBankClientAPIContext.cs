using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
    }
}
