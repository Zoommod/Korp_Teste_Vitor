using FaturamentoService.Application.CasosDeUso;
using FaturamentoService.Application.Interfaces;
using FaturamentoService.Infrastructure.Data;
using FaturamentoService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FaturamentoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
builder.Services.AddScoped<ICriarNotaFiscalUseCase, CriarNotaFiscalUseCase>();
builder.Services.AddScoped<IImprimirNotaFiscalUseCase, ImprimirNotaFiscalUseCase>();

builder.Services.AddHttpClient<IEstoqueClient, EstoqueClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["EstoqueService:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(5);
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
        3,
        retry => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retry))));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.MapControllers();

app.Run();