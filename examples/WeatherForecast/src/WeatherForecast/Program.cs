var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFluentUIComponents();
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var postgreSqlConnectionString = builder.Configuration.GetConnectionString("PostgreSQL");

if (string.IsNullOrWhiteSpace(postgreSqlConnectionString))
{
  // The application configuration does not include a database connection string, use Testcontainers for .NET to create, start and seed the dependent database.
  builder.Services.AddSingleton<DatabaseContainer>();
  builder.Services.AddHostedService(services => services.GetRequiredService<DatabaseContainer>());
  builder.Services.AddDbContext<WeatherDataContext>((services, options) =>
  {
    var databaseContainer = services.GetRequiredService<DatabaseContainer>();
    options.UseNpgsql(databaseContainer.GetConnectionString());
  });
}
else
{
  // The application configuration includes a database connection string, use it to establish a connection and seed the database.
  builder.Services.AddDbContext<WeatherDataContext>((_, options) => options.UseNpgsql(postgreSqlConnectionString));
}

builder.Services.AddScoped<IWeatherDataReadOnlyRepository, WeatherDataReadOnlyContext>();
builder.Services.AddScoped<IWeatherDataWriteOnlyRepository, WeatherDataWriteOnlyContext>();
builder.Services.AddScoped<ISearchCityOrZipCode, SearchCityOrZipCode>();

var app = builder.Build();
app.UseExceptionHandler("/Error");
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();
app.MapControllers();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();

namespace WeatherForecast
{
  public sealed class Program
  {
  }
}
