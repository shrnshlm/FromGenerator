using FromGenerator.Models;
using FromGenerator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.AddScoped<ICLUService, CLUService>();

builder.Services.AddHttpClient<ClaudeService>();
builder.Services.Configure<ClaudeConfig>(builder.Configuration.GetSection("Claude"));
builder.Services.AddScoped<IFormGeneratorService, FormGeneratorService>();
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));
builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<IFormGeneratorService, FormGeneratorService>();
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
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

