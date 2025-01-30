using Microsoft.Extensions.Configuration;
using ActivitiesApi.Services;  // Ensure the correct namespace
using activitiesLibrary;    // הוספת המודל והשירות שלך

namespace ActivitiesApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register DatabaseService as a singleton
            builder.Services.AddSingleton<DatabaseService>();

            // Register UserService as a scoped service
            builder.Services.AddScoped<UserService>();

            // Add HTTP Client
            builder.Services.AddHttpClient("APIClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7250/swagger/index.html");
            });

            // הוספת שירותי הרשאות
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization(); // Ensure Authorization Middleware is used

            app.MapControllers();
            app.Run();


        }
    }
}
