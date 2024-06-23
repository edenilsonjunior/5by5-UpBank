using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UpBank.ClientAPI.Data;
namespace UpBank.ClientAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<UpBankClientAPIContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("UpBankClientAPIContext") ?? throw new InvalidOperationException("Connection string 'UpBankClientAPIContext' not found.")));

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
