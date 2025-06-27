using Microsoft.EntityFrameworkCore;
using PartNumbers.Data;
using PartNumbers.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://staging.d1bfq3uhowhk5h.amplifyapp.com/") // <--- React app local
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddControllers();
builder.Services.AddDbContext<PartNumbersDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PartNumbers")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//app.UseRouting();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
//app.UseSwagger();
//app.UseSwaggerUI();

app.UseCors("AllowLocalhostReact");

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();