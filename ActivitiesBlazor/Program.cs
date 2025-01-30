using ActivitiesBlazor.Components;
using ActivitiesApi.Services;
using System.Runtime.ConstrainedExecution;
namespace ActivitiesBlazor
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<DatabaseService>();
          //  builder.Services.AddScoped<ActivitiesApi.Services.UserService>();
            builder.Services.AddSingleton<UserService>();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Register HttpClient for server-side Blazor
            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}