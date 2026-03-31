using System;
using EstoqueService.Application.DTOs;
using EstoqueService.Application.Interfaces;
using EstoqueService.Application.Mapeamentos;
using EstoqueService.Application.Resultados;
using EstoqueService.Domain.Entities;
using EstoqueService.Domain.Exceptions;

namespace EstoqueService.Application.CasosDeUso;

public class CriarProdutoUseCase : ICriarProdutoUseCase
{
    private readonly IProdutoRepository _repositorio;

    public CriarProdutoUseCase(IProdutoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Resultado<ProdutoDto>> ExecutarAsync(CriarProdutoEntradaDto entrada, CancellationToken cancellationToken)
    {
        if (entrada is null)
            return Resultado<ProdutoDto>.Falha(ErroAplicacao.Validacao("Entrada obrigatória."));

        var existente = await _repositorio.ObterPorCodigoAsync(entrada.Codigo, cancellationToken);
        if (existente is not null)
            return Resultado<ProdutoDto>.Falha(ErroAplicacao.Conflito("Código de produto já existe."));

        try
        {
            var produto = new Produto(entrada.Codigo, entrada.Descricao, entrada.SaldoInicial);

            await _repositorio.AdicionarAsync(produto, cancellationToken);
            await _repositorio.SalvarAlteracoesAsync(cancellationToken);

            return Resultado<ProdutoDto>.Ok(ProdutoMapeador.ParaDto(produto));
        }
        catch (ExcecaoDeDominio ex)
        {
            return Resultado<ProdutoDto>.Falha(ErroAplicacao.Validacao(ex.Message));
        }
    }
}
