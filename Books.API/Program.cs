using Microsoft.Extensions.Configuration;
using Npgsql;
using Constants = Books.API.Models.Constants;

namespace Books.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var config = new ConfigurationBuilder().AddJsonFile(Constants.appSettings).Build();
            var connectionString = config[Constants.connectionString];
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception(nameof(connectionString));

            builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.MapControllers();

            app.Run();
        }
    }
}
