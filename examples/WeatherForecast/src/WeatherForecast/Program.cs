using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Fast.Components.FluentUI;
using WeatherForecast.Interactors.SearchCityOrZipCode;
using WeatherForecast.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddFluentUIComponents();
builder.Services.AddSingleton<IWeatherDataReadOnlyRepository, WeatherDataReadOnlyRepository>();
builder.Services.AddSingleton<IWeatherDataWriteOnlyRepository, WeatherDataWriteOnlyRepository>();
builder.Services.AddSingleton<ISearchCityOrZipCode, SearchCityOrZipCode>();

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
