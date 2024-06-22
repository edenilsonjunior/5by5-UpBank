using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.People;

namespace UpBank.EmployeeAPI.Data
{
    public class UpBankEmployeeAPIContext : DbContext
    {
        public UpBankEmployeeAPIContext (DbContextOptions<UpBankEmployeeAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Models.People.Employee> Employee { get; set; } = default!;
        public object Client { get; internal set; }
        public object Account { get; internal set; }
    }
}
