using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Agencies;
using UpBank.AgencyAPI.Data;
namespace UpBank.AgencyAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<UpBankAgencyAPIContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("UpBankAgencyAPIContext") ?? throw new InvalidOperationException("Connection string 'UpBankAgencyAPIContext' not found.")));

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddSingleton<AgenciesService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
