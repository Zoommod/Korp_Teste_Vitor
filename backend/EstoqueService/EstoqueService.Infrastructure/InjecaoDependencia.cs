using System;

using EstoqueService.Application.Interfaces;
using EstoqueService.Application.CasosDeUso;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EstoqueService.Infrastructure.Data;
using EstoqueService.Infrastructure.Repositories;

namespace EstoqueService.Infrastructure;

public static class InjecaoDependencia
{
    public static IServiceCollection AddEstoqueModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EstoqueSqlServer");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string EstoqueSqlServer nao configurada.");

        services.AddDbContext<EstoqueDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IProdutoRepository, ProdutoRepository>();

        services.AddScoped<ICriarProdutoUseCase, CriarProdutoUseCase>();
        services.AddScoped<IAtualizarProdutoUseCase, AtualizarProdutoUseCase>();
        services.AddScoped<IBuscarProdutoPorIdUseCase, BuscarProdutoPorIdUseCase>();
        services.AddScoped<IListarProdutosUseCase, ListarProdutosUseCase>();
        services.AddScoped<IAbaterEstoqueUseCase, AbaterEstoqueUseCase>();

        return services;
    }
}
