using ahis.template.application.Services;
using ahis.template.identity;
using ahis.template.infrastructure;
using ahis.template.infrastructure.Contexts;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IEmailSender, EmailSender>();


// Add assemblies service extentions
builder.Services.AddInfrastructure();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentityServices(connectionString);

builder.Services.AddControllers();

// CORS: configure a named policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
            .WithOrigins("https://localhost:7270", "https://localhost:7280") // <-- allowed origins
            .AllowAnyHeader()
            .AllowAnyMethod();
            //.AllowCredentials(); // remove if do not need cookies/credentials
    });

    // DEV: you can add an open policy for development only (optional)
    options.AddPolicy("AllowAllDev", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANT: apply CORS before authorization and endpoint mapping
app.UseCors("DefaultCorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
