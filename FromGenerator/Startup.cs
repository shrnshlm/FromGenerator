using FromGenerator.Models;
using FromGenerator.Services;
using FromGenerator.Configuration;

namespace FromGenerator;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Configure Claude settings
        services.Configure<ClaudeSettings>(Configuration.GetSection("Claude"));
        services.AddHttpClient<ClaudeService>();
        services.AddScoped<IClaudeService, ClaudeService>();
        services.AddScoped<IFormGeneratorService, FormGeneratorService>();
        services.Configure<EmailConfig>(Configuration.GetSection("EmailConfig"));
        services.AddScoped<EmailService>();

        // Add CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(
                        "http://localhost:5173",
                        "http://formgenerator-frontend-2025.s3-website-us-west-2.amazonaws.com"
                      )
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors("AllowFrontend");

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}