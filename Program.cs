using ReleaseNotesGenerator;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog can check logs on output window
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()    
    .CreateLogger();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAzureOpenAIClient(builder.Configuration);
builder.Services.AddGitHubClient(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("https://example.com") // Replace with your allowed origin(s)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowSpecificOrigin");
app.MapControllers();

app.Run();
