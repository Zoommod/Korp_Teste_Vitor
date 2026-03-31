using System;
using EstoqueService.Application.DTOs;
using EstoqueService.Application.Interfaces;
using EstoqueService.Application.Mapeamentos;
using EstoqueService.Application.Resultados;

namespace EstoqueService.Application.CasosDeUso;

public class BuscarProdutoPorIdUseCase : IBuscarProdutoPorIdUseCase
{
    private readonly IProdutoRepository _repositorio;

    public BuscarProdutoPorIdUseCase(IProdutoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Resultado<ProdutoDto>> ExecutarAsync(Guid id, CancellationToken cancellationToken)
    {
        var produto = await _repositorio.ObterPorIdAsync(id, cancellationToken);
        if(produto is null)
            return Resultado<ProdutoDto>.Falha(ErroAplicacao.NaoEncontrado("Produto não encontrado"));
        
        return Resultado<ProdutoDto>.Ok(ProdutoMapeador.ParaDto(produto));
    }

}
