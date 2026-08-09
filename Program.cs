

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using swacv_api.Data;
using swacv_api.Services;

var builder = WebApplication.CreateBuilder(args);


// ======================================================
// CORS
// ======================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});


// ======================================================
// BASE DE DATOS
// ======================================================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


// ======================================================
// SERVICIOS DE AUTENTICACIÓN
// ======================================================

builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<TokenService>();


// ======================================================
// CONFIGURACIÓN JWT
// ======================================================

string jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "No se encontró la configuración Jwt:Key.");

string jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "No se encontró la configuración Jwt:Issuer.");

string jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "No se encontró la configuración Jwt:Audience.");

byte[] jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);

if (jwtKeyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "La clave JWT debe tener al menos 32 bytes.");
}


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // Validar quién generó el token
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                // Validar para qué aplicación fue generado
                ValidateAudience = true,
                ValidAudience = jwtAudience,

                // Validar la fecha de expiración
                ValidateLifetime = true,

                // Validar la firma del token
                ValidateIssuerSigningKey = true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(jwtKeyBytes),

                // El token deja de ser válido exactamente
                // cuando llegue su fecha de expiración.
                ClockSkew = TimeSpan.Zero
            };
    });


builder.Services.AddAuthorization();


// ======================================================
// CONTROLADORES Y SWAGGER
// ======================================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


var app = builder.Build();


// ======================================================
// PIPELINE HTTP
// ======================================================

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

// Primero determina quién es el usuario.
app.UseAuthentication();

// Después determina qué puede hacer.
app.UseAuthorization();

app.MapControllers();


// ======================================================
// ENDPOINT DE EJEMPLO
// ======================================================

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =
        Enumerable.Range(1, 5)
            .Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(
                        DateTime.Now.AddDays(index)
                    ),
                    Random.Shared.Next(-20, 55),
                    summaries[
                        Random.Shared.Next(
                            summaries.Length
                        )
                    ]
                ))
            .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


app.Run();


record WeatherForecast(
    DateOnly Date,
    int TemperatureC,
    string? Summary)
{
    public int TemperatureF =>
        32 + (int)(TemperatureC / 0.5556);
}