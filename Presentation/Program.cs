using System.Text.Json.Serialization;
using Business.Service;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Repository;
using FluentAssertions.Common;
using MailKit;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Models.Entities;

namespace Presentation;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<CoffeeContext>(opts =>
            opts.UseSqlServer(
                builder.Configuration.GetConnectionString("CoffeeshopDB_Silas")));
        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddLogging();
        builder.Services.AddCors();
        
        // Services
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICoffeeService, CoffeeService>();
        builder.Services.AddScoped<IAdditionService, AdditionService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IEmailSender, EmailSender>();

        
        
        //Repositories
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ICoffeeRepository, CoffeeRepository>();
        builder.Services.AddScoped<IAdditionRepository, AdditionRepository>();
        builder.Services.AddScoped<ICommentRepository, CommentRepository>();
        builder.Services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c=> 
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Example API",
                Version = "v1",
                Description = "An example of an ASP.NET Core API",
                Contact = new OpenApiContact()
                {
                    Name = "Example Contact",
                    Email = "example@example.com",
                    Url = new Uri("https://example.com/contact"),
                },
            });
        });
        builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        }

        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
        app.UseRouting();
        //app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}