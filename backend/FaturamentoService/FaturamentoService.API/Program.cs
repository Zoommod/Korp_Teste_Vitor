using FaturamentoService.API.Services;
using FaturamentoService.Application.CasosDeUso;
using FaturamentoService.Application.Interfaces;
using FaturamentoService.Infrastructure.Data;
using FaturamentoService.Infrastructure.Mensageria;
using FaturamentoService.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FaturamentoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

builder.Services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
builder.Services.AddScoped<ICriarNotaFiscalUseCase, CriarNotaFiscalUseCase>();
builder.Services.AddScoped<IImprimirNotaFiscalUseCase, ImprimirNotaFiscalUseCase>();
builder.Services.AddScoped<IEstoqueMessagePublisher, EstoqueMessagePublisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();