using Microsoft.EntityFrameworkCore;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Repositories;
using ERP.Application.Interfaces;
using ERP.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAÇÃO DE SERVIÇOS ---

builder.Services.AddControllers(); 

// Conexão com PostgreSQL via AppDbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configuração de CORS para o Frontend Next.js (Porta 3000)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Swagger para testes da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Injeção de Dependência - Camadas do ERP
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<CreateTenantUseCase>();
// Se tiver Repositórios de Produtos e Usuários, adicione-os aqui também:
// builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

// --- 2. CONFIGURAÇÃO DO MIDDLEWARE ---

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers(); 

app.MapGet("/", () => "API do ERP Modularis está rodando com PostgreSQL!");

app.Run();