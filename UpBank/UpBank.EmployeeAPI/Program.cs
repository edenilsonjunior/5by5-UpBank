using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UpBank.EmployeeAPI.Data;
namespace UpBank.EmployeeAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<UpBankEmployeeAPIContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("UpBankEmployeeAPIContext") ?? throw new InvalidOperationException("Connection string 'UpBankEmployeeAPIContext' not found.")));

            // Add services to the container.

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
