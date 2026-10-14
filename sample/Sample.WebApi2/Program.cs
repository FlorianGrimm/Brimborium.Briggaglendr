namespace Sample.WebApi2;

public class Program {
    public readonly WebApplicationBuilder Builder;
    public readonly AppConfiguration AppConfiguration;
    private WebApplication? _WebApplication;
    public WebApplication WebApplication => this._WebApplication ?? throw new Exception("WebApplication is null");

    public Program(WebApplicationBuilder builder, AppConfiguration appConfiguration) {
        this.Builder = builder;
        this.AppConfiguration = appConfiguration;
    }

    public async Task MainAsync() {
        try {
            await this.RunAsync();
        } catch (AggregateException error) {
            error.Handle((error) => {
                System.Console.Error.WriteLine(error.ToString());
                return true;
            });
        } catch (Exception error) {
            System.Console.Error.WriteLine(error.ToString());
        }
    }
    public virtual async Task RunAsync() {
        this.ConfigureBuilder();
        var app = this.Builder.Build();
        this._WebApplication = app;
        this.ConfigureApplication();
        await app.RunAsync();
    }

    private void ConfigureBuilder() {
        this.Builder.Services.AddRazorPages();
        this.Builder.Services.AddBriggaglendr(
            this.Builder.Configuration.GetSection("Briggaglendr"),
            (options) => {
                //
            });
    }

    private void ConfigureApplication() {
        var app = this.WebApplication;
        app.UseBriggaglendr();
        app.MapOpenApi();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.MapBriggaglendr();

        this.ConfigureMap();
    }

    private void ConfigureMap() {

        var app = this.WebApplication;

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        _ = app.MapGet("/weatherforecast", () => {
            return forecast;
        })
        .WithName("GetWeatherForecast");
    }

    public static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        AppConfiguration appConfiguration = new();
        builder.Configuration.Bind(appConfiguration);

        Program program = new(builder, appConfiguration);
        await program.MainAsync();
    }
}

public class AppConfiguration {
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary) {
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
