using System.Text;
using AvaloniaSimpleChat.Server.Hubs;
using AvaloniaSimpleChat.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

namespace AvaloniaSimpleChat.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddSignalR();
        builder.Services.AddControllers();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<ISecurityService, SecurityService>();
        
        // Configure JWT Authentication
        SecurityService.ConfigureJwtBearerOptions(builder);

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapHub<ChatHub>("/api/chatHub");
        app.MapControllers();

        app.Run();
    }
}