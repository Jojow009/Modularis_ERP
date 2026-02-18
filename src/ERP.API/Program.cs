using Microsoft.EntityFrameworkCore;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURAÇÃO DO BANCO DE DADOS ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- INJEÇÃO DE DEPENDÊNCIA ---
builder.Services.AddScoped<ProductRepository>();

// --- CONFIGURAÇÃO DO REDIS ---
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost:6379";
});

// --- NOVO: CONFIGURAÇÃO DE CORS ---
// Permite que o seu frontend Next.js converse com esta API
builder.Services.AddCors(options =>
{
    options.AddPolicy("ModularisAppPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Origem do seu frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- SEGURANÇA: CONFIGURAÇÃO DO JWT ---
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ChaveSegurancaPadraoModularis2026";
var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; 
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false, 
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- ORDEM DOS MIDDLEWARES (CRUCIAL) ---

// 1. O CORS deve vir antes de tudo para autorizar a requisição do navegador
app.UseCors("ModularisAppPolicy");

// 2. Autenticação (Quem é você?)
app.UseAuthentication(); 

// 3. Autorização (O que você pode fazer?)
app.UseAuthorization();  

app.MapControllers();
app.Run();