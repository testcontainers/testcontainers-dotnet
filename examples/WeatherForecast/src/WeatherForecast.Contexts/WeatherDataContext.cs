namespace WeatherForecast.Contexts;

[PublicAPI]
public sealed class WeatherDataContext : DbContext
{
  public WeatherDataContext(DbContextOptions<WeatherDataContext> options) : base(options)
  {
  }

  public DbSet<WeatherData> WeatherData { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // Map all read-only properties. There is not [Mapped] attribute like [NotMapped].
    modelBuilder.Entity<WeatherData>().Property(weatherData => weatherData.Id);
    modelBuilder.Entity<WeatherData>().Property(weatherData => weatherData.Date);
    modelBuilder.Entity<Temperature>().Property(temperature => temperature.Id);
    modelBuilder.Entity<Temperature>().Property(temperature => temperature.UnitName);
    modelBuilder.Entity<Temperature>().Property(temperature => temperature.UnitSymbol);
    modelBuilder.Entity<Temperature>().Property(temperature => temperature.Value);
    modelBuilder.Entity<Temperature>().Property(temperature => temperature.Measured);
    modelBuilder.Entity<WeatherData>().HasMany(weatherData => weatherData.Temperatures).WithOne().HasForeignKey(temperature => temperature.BelongsTo);

    var weatherDataSeed = Enumerable.Range(0, 30).Select(_ => Guid.NewGuid()).Select((id, day) => new WeatherData(id, DateTime.Today.AddDays(day))).ToList();
    var temperatureSeed = weatherDataSeed.SelectMany(data => Enumerable.Range(0, 23).Select(hour => Temperature.Celsius(data.Id, Random.Shared.Next(-10, 30), data.Date.AddHours(hour)))).ToList();

    modelBuilder.Entity<Temperature>().HasData(temperatureSeed);
    modelBuilder.Entity<WeatherData>().HasData(weatherDataSeed);
    base.OnModelCreating(modelBuilder);
  }
}
