using FromGenerator.Models;
using FromGenerator.Services;
using FromGenerator.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.AddScoped<ICLUService, CLUService>();

// Configure Claude settings
builder.Services.Configure<ClaudeSettings>(builder.Configuration.GetSection("Claude"));
builder.Services.AddHttpClient<ClaudeService>();
builder.Services.AddScoped<IClaudeService, ClaudeService>();
builder.Services.AddScoped<IFormGeneratorService, FormGeneratorService>();
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));
builder.Services.AddScoped<EmailService>();
// Add CORS
builder.Services.AddCors(options =>
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
var app = builder.Build();
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

