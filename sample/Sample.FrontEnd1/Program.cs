namespace Sample.FrontEnd1;

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
        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();
    }

    public static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        AppConfiguration appConfiguration = new();
        builder.Configuration.Bind(appConfiguration);

        Program program = new (builder, appConfiguration);
        await program.MainAsync();
    }
}

public class AppConfiguration { 
}