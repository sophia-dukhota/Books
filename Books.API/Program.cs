using Npgsql;

namespace Books.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        string connectionString = string.Empty;

        if (File.Exists(Constants.CONNECTIONSTRING_FILELOCATION))
        {
            connectionString = File.ReadAllText(Constants.CONNECTIONSTRING_FILELOCATION);
        }

        else
        {
            throw new Exception(nameof(Constants.CONNECTIONSTRING_FILELOCATION));
        }

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