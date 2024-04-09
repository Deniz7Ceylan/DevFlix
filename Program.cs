using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DevFlix.Data;
using DevFlix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CodeFixes;

namespace DevFlix;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<DevFlixContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DevFlixContext") ?? throw new InvalidOperationException("Connection string 'DevFlixContext' not found.")));

        builder.Services.AddIdentity<DevFlixUser, DevFlixRole>().AddDefaultTokenProviders()
                 .AddEntityFrameworkStores<DevFlixContext>();


        // Add services to the container.

        builder.Services.AddControllers().AddNewtonsoftJson(opt =>
        {
            opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        DevFlixContext? context = app.Services.CreateScope().ServiceProvider.GetService<DevFlixContext>();
        RoleManager<DevFlixRole>? roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<DevFlixRole>>();
        UserManager<DevFlixUser>? userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<DevFlixUser>>();
        DBInitializer dBInitializer = new DBInitializer(context, roleManager, userManager);

        app.Run();
    }
}
