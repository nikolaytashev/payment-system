using Identity.API;
using Identity.API.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("Default");
var migrationAssembly = typeof(Program).Assembly.GetName().Name;

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer(options =>
    {
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseSuccessEvents = true;
        options.Events.RaiseFailureEvents = true;

        options.EmitStaticAudienceClaim = true;
    })
    .AddConfigurationStore(opts =>
    {
        opts.ConfigureDbContext = x => x.UseSqlite(connectionString, z =>
        {
            z.MigrationsAssembly(migrationAssembly);
        });
    })
    .AddOperationalStore(opts =>
    {
        opts.ConfigureDbContext = x => x.UseSqlite(connectionString, z =>
        {
            z.MigrationsAssembly(migrationAssembly);
        });
    })
    .AddAspNetIdentity<IdentityUser>();
builder.Services.AddAuthentication();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlite(connectionString, z =>
    {
        z.MigrationsAssembly(migrationAssembly);
    });
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

DataSeeder.MigrateDatabase(app);
await DataSeeder.SeedData(app);

app.MapGet("/", () => "Identity server");

app.MapControllers();

app.UseIdentityServer();
app.UseAuthentication();

app.Run();
