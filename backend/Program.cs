using Microsoft.EntityFrameworkCore;
using Cstream.Data;
using Cstream.Hubs;
using Cstream.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSignalR();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<CstreamContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("CstreamContextSQLite")));
builder.Services.AddScoped<UserService>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

if (builder.Environment.IsDevelopment())
{

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("SignalRCorsPolicy", policy =>
        {
            policy.WithOrigins("https://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });
}
var app = builder.Build();

// using static files
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
    app.MapOpenApi();
    app.UseCors("SignalRCorsPolicy");

}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<CstreamContext>();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<VideoStreamHub>("/stream");

app.Run();
