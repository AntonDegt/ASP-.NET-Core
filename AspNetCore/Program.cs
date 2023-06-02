using AspNetCore.Controllers;
using AspNetCore.Data;
using AspNetCore.Services;
using AspNetCore.Services.Hash;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddSingleton<DateService>();
builder.Services.AddSingleton<IDateService, DateService>();
builder.Services.AddSingleton<IHashService, Md5HashService>();

builder.Services.AddScoped<TimeService>();
builder.Services.AddTransient<DateTimeService>();


string? connectionString = builder.Configuration.GetConnectionString("PlanetScale");
MySqlConnection connection = new(connectionString);
builder.Services.AddDbContext<DataContext>(
    options =>
        options.UseMySql(
               connection,
               ServerVersion.AutoDetect(connection),
               ServerOptions =>
               ServerOptions.MigrationsHistoryTable(
                   tableName: HistoryRepository.DefaultTableName,
                   schema: "asp")
               .SchemaBehavior(
                   MySqlSchemaBehavior.Translate,
                   (schema, table) => $"{schema}_{table}")
 ));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
