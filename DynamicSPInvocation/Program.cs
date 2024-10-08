using DynamicSPInvocation.Interface;
using DynamicSPInvocation.Model;
using DynamicSPInvocation.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Debugging;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
 .SetBasePath(Directory.GetCurrentDirectory())
 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
 .Build();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "your-issuer",
        ValidAudience = "your-audience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret-key"))
    };
});


SelfLog.Enable(msg =>
{
    Console.WriteLine(msg); // Log errors to the console
    System.IO.File.AppendAllText("serilog-errors.txt", msg + Environment.NewLine); // Also log errors to a file
});
configuration["Serilog:WriteTo:0:Args:connectionString"] = configuration["Serilog:WriteTo:0:Args:connectionString"];

Log.Logger = new LoggerConfiguration()
 .ReadFrom.Configuration(configuration).Enrich.FromLogContext()
    .CreateLogger();





builder.Host.UseSerilog(((ctx, lc) => {
    var str = ctx.Configuration["Serilog:WriteTo:0:Args:connectionString"];
    ctx.Configuration["Serilog:WriteTo:0:Args:connectionString"] = str;
    lc.ReadFrom.Configuration(ctx.Configuration);
}));


//builder.Host.UseSerilog();
// Add services to the container.
builder.Services.AddTransient<IHandlingMultipleSP, DynamicSPService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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

app.Run();
Log.CloseAndFlush();