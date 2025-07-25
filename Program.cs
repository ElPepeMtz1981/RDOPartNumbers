using Microsoft.EntityFrameworkCore;
using RDOXMES.PartNumbers;

Console.WriteLine("*******");
Console.WriteLine("*******");
Console.WriteLine();
Console.WriteLine("Start Program.cs");

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

//var apiPortString = Environment.GetEnvironmentVariable("API_PORT") ?? "5000";
//Console.WriteLine($"API_PORT: {apiPortString}");
//var port = int.Parse(apiPortString);

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenLocalhost(port);
//});

if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("Development");
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5000); // HTTP
        options.ListenLocalhost(5001, listenOptions =>
        {
            listenOptions.UseHttps(); // Certificado de desarrollo
        });
    });
}
else
{
    Console.WriteLine("Production");
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

var kestrelSection = builder.Configuration.GetSection("Kestrel");
if (kestrelSection.Exists())
{
    Console.WriteLine("Kestrel configuration found via IConfiguration:");
    foreach (var child in kestrelSection.GetChildren())
    {
        Console.WriteLine($"{child.Key}: {child.Value}");
    }
}
else
{
    Console.WriteLine("No Kestrel configuration found in IConfiguration.");
}

var endpointsSection = builder.Configuration.GetSection("Kestrel:Endpoints");
foreach (var endpoint in endpointsSection.GetChildren())
{
    Console.WriteLine($"Endpoint: {endpoint.Key}");
    foreach (var setting in endpoint.GetChildren())
    {
        Console.WriteLine($"  {setting.Key}: {setting.Value}");
    }
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