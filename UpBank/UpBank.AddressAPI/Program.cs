using Microsoft.Extensions.Options;
using Services.People;
using Services.Utils;

namespace UpBank.AddressAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            //---------------------------
            builder.Services.Configure<MongoDataBaseSettings>(
               builder.Configuration.GetSection(nameof(MongoDataBaseSettings)));

            builder.Services.AddSingleton<IMongoDataBaseSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoDataBaseSettings>>().Value);

            builder.Services.AddSingleton<AddressService>();
            //---------------------------

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
