using EstoqueService.Application.Consumidores;
using EstoqueService.Infrastructure;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEstoqueModule(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<NotaFiscalImpressaConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("estoque-baixa-nota-fiscal", e =>
        {
            e.ConfigureConsumer<NotaFiscalImpressaConsumer>(context);
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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