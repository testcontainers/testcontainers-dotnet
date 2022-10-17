using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Fast.Components.FluentUI;
using WeatherForecast.Contexts;
using WeatherForecast.Interactors.SearchCityOrZipCode;
using WeatherForecast.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddFluentUIComponents();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
  builder.Services.AddSingleton<IWeatherDataReadOnlyRepository, WeatherDataReadOnlyRepository>();
  builder.Services.AddSingleton<IWeatherDataWriteOnlyRepository, WeatherDataWriteOnlyRepository>();
  builder.Services.AddSingleton<ISearchCityOrZipCode, SearchCityOrZipCode>();
}
else
{
  builder.Services.AddDbContext<WeatherDataContext>(options => options.UseSqlServer(connectionString));
  builder.Services.AddScoped<IWeatherDataReadOnlyRepository, WeatherDataReadOnlyContext>();
  builder.Services.AddScoped<IWeatherDataWriteOnlyRepository, WeatherDataWriteOnlyContext>();
  builder.Services.AddScoped<ISearchCityOrZipCode, SearchCityOrZipCode>();
}

var app = builder.Build();
app.UseExceptionHandler("/Error");
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();
app.UseEndpoints(endpoint => endpoint.MapControllers());
app.UseEndpoints(endpoint => endpoint.MapRazorPages());
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();

public sealed partial class Program
{
}
