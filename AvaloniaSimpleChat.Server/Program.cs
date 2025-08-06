using System.Text;
using AvaloniaSimpleChat.Server.Hubs;
using AvaloniaSimpleChat.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
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
        
        // Add CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .AllowAnyOrigin()     // Allow requests from any origin
                    .AllowAnyMethod()     // Allow any HTTP method (GET, POST, etc.)
                    .AllowAnyHeader()     // Allow any HTTP headers
                    .WithExposedHeaders("*");  // Expose all headers to the client
            });
        });
        
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

        
        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings[".wasm"] = "application/wasm";
        provider.Mappings[".dll"] = "application/octet-stream";
        provider.Mappings[".dat"] = "application/octet-stream";
        provider.Mappings[".blat"] = "application/octet-stream";
        provider.Mappings[".json"] = "application/json";
        provider.Mappings[".js"] = "application/javascript";

        app.UseDefaultFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = provider,
            ServeUnknownFileTypes = true // This ensures files with unknown extensions are served
        });


        app.MapHub<ChatHub>("/api/chatHub");
        app.MapControllers();

        app.Run();
    }
}