using Microsoft.EntityFrameworkCore;
using RDOXMES.PartNumbers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddDbContext<PartNumbersDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PartNumbers")));
builder.Services.AddEndpointsApiExplorer();

if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5000); // HTTP
        options.ListenLocalhost(5001, listenOptions =>
        {
            listenOptions.UseHttps(); // Certificado de desarrollo
        });
    });
}

builder.Services.AddSwaggerGen();

var connection = builder.Configuration.GetConnectionString("PartNumbers");
if (string.IsNullOrWhiteSpace(connection) || connection.Contains("USE_ENV_VARIABLE"))
{
    builder.Logging.AddConsole();
    Console.WriteLine("String connection not loaded.");
}
else
{
    Console.WriteLine($"String connection: {connection}");
}

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Contains("USE_ENV_VARIABLE"))
{
    builder.Logging.AddConsole();
    Console.WriteLine("JWT not loaded.");
}
else
{
    Console.WriteLine($"JWT: {jwtKey}");
}

var urls = builder.Configuration["ASPNETCORE_URLS"];
if (string.IsNullOrWhiteSpace(urls) || urls.Contains("USE_ENV_VARIABLE"))
{
    builder.Logging.AddConsole();
    Console.WriteLine("ASPNETCORE_URLS not loaded.");
}
else
{
    Console.WriteLine($"ASPNETCORE_URLS: {urls}");
}

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

//app.UseRouting();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();