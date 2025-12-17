using ahis.template.infrastructure.Contexts;
using ahis.template.infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add assemblies service extentions
builder.Services.AddInfrastructure();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

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
builder.Services.AddSwaggerGen();

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
