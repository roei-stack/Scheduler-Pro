using Microsoft.EntityFrameworkCore;
using server;
using server.Data;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//in memory database
builder.Services.AddDbContext<UsersAPIDbContext>(options => options.UseInMemoryDatabase("SchedulerDb"));

// sql database
// Add-Migration init
// Update-Database
/*builder.Services.AddDbContext<UsersAPIDbContext>(options => 
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDb")));*/


builder.Services.AddSingleton<KeyManager>();

builder.Services.AddCors(options => 
    options.AddDefaultPolicy(builder => {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    })
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var secretKey = builder.Services.BuildServiceProvider().GetRequiredService<KeyManager>().GetSecretKey();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = "Scheduler-Pro",
        ValidAudience = "Scheduler-Pro",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
