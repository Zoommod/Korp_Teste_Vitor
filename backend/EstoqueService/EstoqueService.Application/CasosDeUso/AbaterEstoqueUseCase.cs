using System;
using EstoqueService.Application.DTOs;
using EstoqueService.Application.Interfaces;
using EstoqueService.Application.Resultados;
using EstoqueService.Domain.Entities;
using EstoqueService.Domain.Exceptions;

namespace EstoqueService.Application.CasosDeUso;

public sealed class AbaterEstoqueUseCase : IAbaterEstoqueUseCase
{
    private readonly IProdutoRepository _repositorio;

    public AbaterEstoqueUseCase(IProdutoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Resultado<AbaterEstoqueResultadoDto>> ExecutarAsync(AbaterEstoqueEntradaDto entrada, CancellationToken cancellationToken)
    {
        if (entrada?.Itens is null || entrada.Itens.Count == 0)
            return Resultado<AbaterEstoqueResultadoDto>.Falha(ErroAplicacao.Validacao("Lista de itens obrigatória."));

        var itensAgrupados = entrada.Itens.GroupBy(i => i.CodigoProduto.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(i => i.Quantidade),
                StringComparer.OrdinalIgnoreCase);

        foreach (var par in itensAgrupados)
        {
            if (string.IsNullOrWhiteSpace(par.Key))
                return Resultado<AbaterEstoqueResultadoDto>.Falha(ErroAplicacao.Validacao("Código do produto obrigatório."));

            if (par.Value <= 0)
                return Resultado<AbaterEstoqueResultadoDto>.Falha(ErroAplicacao.Validacao("Quantidade deve ser maior que zero."));
        }

        var produtosPorCodigo = new Dictionary<string, Produto>(StringComparer.OrdinalIgnoreCase);

        foreach (var par in itensAgrupados)
        {
            var produto = await _repositorio.ObterPorCodigoAsync(par.Key, cancellationToken);

            if (produto is null)
            {
                return Resultado<AbaterEstoqueResultadoDto>.Falha(ErroAplicacao.NaoEncontrado($"Produto não encontrado: {par.Key}."));
            }

            if (produto.Saldo - par.Value < 0)
            {
                return Resultado<AbaterEstoqueResultadoDto>.Falha(ErroAplicacao.Validacao($"Saldo insuficiente para o produto: {par.Key}."));
            }

            produtosPorCodigo[par.Key] = produto;
        }

        try
        {
            foreach (var par in itensAgrupados) produtosPorCodigo[par.Key].AbaterEstoque(par.Value);

            await _repositorio.SalvarAlteracoesAsync(cancellationToken);

            var resultado = new AbaterEstoqueResultadoDto(itensAgrupados
            .Select(par => new ItemSaldoDto(par.Key, produtosPorCodigo[par.Key].Saldo))
            .ToList()
            .AsReadOnly());

            return Resultado<AbaterEstoqueResultadoDto>.Ok(resultado);
        }
        catch (ExcecaoDeDominio ex)
        {
            return Resultado<AbaterEstoqueResultadoDto>.Falha(ErroAplicacao.Validacao(ex.Message));
        }
    }
}
