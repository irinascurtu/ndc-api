
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductsApi.Data;
using ProductsApi.Data.Repositories;
using ProductsApi.Infrastructure.Mappings;
using ProductsApi.Service;

namespace ProductsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers(option =>
            {
                option.RespectBrowserAcceptHeader = true;
            });

            builder.Services.AddAutoMapper(typeof(ProductProfileMapping).Assembly);


            //builder.Services.AddDbContext<ProductContext>(options =>
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDbContext<ProductContext>(options =>
                 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                    .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                    .EnableSensitiveDataLogging());

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddSingleton<IMemoryCache>(new MemoryCache(
              new MemoryCacheOptions
              {
                  TrackStatistics = true,
                  SizeLimit = 50 // Products.
              }));

            builder.Services.AddResponseCaching();

            builder.Services.AddDistributedMemoryCache();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseDeveloperExceptionPage();
                using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<ProductContext>().Database.EnsureCreated();
                    serviceScope.ServiceProvider.GetService<ProductContext>().EnsureSeeded();
                }
            }


            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseResponseCaching();

            app.MapControllers();

            app.Run();
        }
    }
}
