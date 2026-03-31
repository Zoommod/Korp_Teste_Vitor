using System;
using EstoqueService.Application.DTOs;
using EstoqueService.Application.Interfaces;
using EstoqueService.Application.Mapeamentos;
using EstoqueService.Application.Resultados;

namespace EstoqueService.Application.CasosDeUso;

public sealed class ListarProdutosUseCase : IListarProdutosUseCase
{
    private readonly IProdutoRepository _repositorio;

    public ListarProdutosUseCase(IProdutoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Resultado<IReadOnlyList<ProdutoDto>>> ExecutarAsync(CancellationToken cancellationToken)
    {
        var produtos = await _repositorio.ListarAsync(cancellationToken);

        var dtos = produtos
            .Select(ProdutoMapeador.ParaDto)
            .ToList()
            .AsReadOnly();

        return Resultado<IReadOnlyList<ProdutoDto>>.Ok(dtos);
    }
}
