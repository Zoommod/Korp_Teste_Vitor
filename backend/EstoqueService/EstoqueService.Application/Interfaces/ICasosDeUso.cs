using System;
using EstoqueService.Application.DTOs;
using EstoqueService.Application.Resultados;

namespace EstoqueService.Application.Interfaces;

public interface ICriarProdutoUseCase
{
    Task<Resultado<ProdutoDto>> ExecutarAsync(CriarProdutoEntradaDto entrada, CancellationToken cancellationToken);
}

public interface IAtualizarProdutoUseCase
{
    Task<Resultado<ProdutoDto>> ExecutarAsync(AtualizarProdutoEntradaDto entrada, CancellationToken cancellationToken);
}

public interface IBuscarProdutoPorIdUseCase
{
    Task<Resultado<ProdutoDto>> ExecutarAsync(Guid id, CancellationToken cancellationToken);
}

public interface IListarProdutosUseCase
{
    Task<Resultado<IReadOnlyList<ProdutoDto>>> ExecutarAsync(CancellationToken cancellationToken);
}

public interface IAbaterEstoqueUseCase
{
    Task<Resultado<AbaterEstoqueResultadoDto>> ExecutarAsync(AbaterEstoqueEntradaDto entrada, CancellationToken cancellationToken);
}
