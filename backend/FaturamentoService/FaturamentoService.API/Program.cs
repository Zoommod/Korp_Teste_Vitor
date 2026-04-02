using FaturamentoService.API.Services;
using FaturamentoService.Application.CasosDeUso;
using FaturamentoService.Application.Interfaces;
using FaturamentoService.Infrastructure.Data;
using FaturamentoService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FaturamentoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
builder.Services.AddScoped<ICriarNotaFiscalUseCase, CriarNotaFiscalUseCase>();
builder.Services.AddScoped<IImprimirNotaFiscalUseCase, ImprimirNotaFiscalUseCase>();
builder.Services.AddScoped<IEstoqueMessagePublisher, FakeEstoqueMessagePublisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();