using Core7_RBS_minimalAPI.Models;
using Core7_RBS_minimalAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SecurityDbContext>(options => 
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SecurityConnStr"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    // USe the EF Core for CReating, Managing USers and Roles
    .AddEntityFrameworkStores<SecurityDbContext>();

builder.Services.AddScoped<SecurityServices>();
builder.Services.AddAuthentication();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("read", policy => policy.RequireRole("Manager", "Clerk"));


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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapPost("/registeruser", async (RegisterUser user, SecurityServices serv) => { 
     var response = await serv.RegisterUserAsync(user);
     return Results.Ok(response);
});

app.MapPost("/loginuser", async (LoginUser user, SecurityServices serv) => {
    var response = await serv.AuthUser(user);
    return Results.Ok(response);
});

app.MapPost("/createrole", async (RoleData role, SecurityServices serv) => {
    var response = await serv.CreateRoleAsync(role);
    return Results.Ok(response);
});


app.MapPost("/assigrole", async (UserRole userrole, SecurityServices serv) => {
    var response = await serv.AddRoleToUserAsync(userrole);
    return Results.Ok(response);
});


app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi()
.RequireAuthorization("read");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
