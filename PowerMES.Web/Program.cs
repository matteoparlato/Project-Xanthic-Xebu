using IO.Swagger.Api;
using IO.Swagger.Client;
using PowerMES.Web.Components;

namespace PowerMES.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IDefaultApi>(x =>
        {
            var apiConfig = builder.Configuration.GetSection("API");
            return new DefaultApi(new Configuration()
            {
                BasePath = builder.Configuration.GetValue<string>("powermes-web-hostname"),
                Username = builder.Configuration.GetValue<string>("powermes-web-username"),
                Password = builder.Configuration.GetValue<string>("powermes-web-password")
            });
        });

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddBlazorBootstrap();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
