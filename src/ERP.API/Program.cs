using Microsoft.EntityFrameworkCore;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Repositories;
using ERP.Application.Interfaces;
using ERP.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVIÇOS ---
// Habilita a API a encontrar o seu TenantsController
builder.Services.AddControllers(); 

// Configura um banco temporário na RAM (Não precisa de string de conexão agora)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ERPDb"));

// Permite que o Next.js (Porta 3000) converse com esta API
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra as camadas para que o Controller possa usá-las
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<CreateTenantUseCase>();

var app = builder.Build();

// --- 2. MIDDLEWARE ---
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ESSENCIAL: Isso ativa a rota /api/tenants
app.MapControllers(); 

app.MapGet("/", () => "API do ERP Modularis está rodando!");

app.Run();