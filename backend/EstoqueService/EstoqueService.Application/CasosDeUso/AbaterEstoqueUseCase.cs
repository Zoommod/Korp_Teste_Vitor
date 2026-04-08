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
    private readonly ITransacaoEstoque _transacao;

    public AbaterEstoqueUseCase(IProdutoRepository repositorio, ITransacaoEstoque transacao)
    {
        _repositorio = repositorio;
        _transacao = transacao;
    }

    public async Task<Resultado<AbaterEstoqueResultadoDto>> ExecutarAsync(AbaterEstoqueEntradaDto entrada, CancellationToken cancellationToken)
    {
        if (entrada?.Itens is null || entrada.Itens.Count == 0)
            return Resultado<AbaterEstoqueResultadoDto>.Falha(
                ErroAplicacao.Validacao("Lista de itens obrigatória."));

        var itensAgrupados = entrada.Itens
            .GroupBy(i => i.CodigoProduto.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(i => i.Quantidade),
                StringComparer.OrdinalIgnoreCase);

        foreach (var par in itensAgrupados)
        {
            if (string.IsNullOrWhiteSpace(par.Key))
                return Resultado<AbaterEstoqueResultadoDto>.Falha(
                    ErroAplicacao.Validacao("Código do produto obrigatório."));

            if (par.Value <= 0)
                return Resultado<AbaterEstoqueResultadoDto>.Falha(
                    ErroAplicacao.Validacao("Quantidade deve ser maior que zero."));
        }

        var saldosAtualizados = new List<ItemSaldoDto>();

        try
        {
            await _transacao.ExecutarAsync(async ct =>
            {
                var codigos = itensAgrupados.Keys.ToList();
                var produtos = await _repositorio.ObterPorCodigosAsync(codigos, ct);

                var produtosPorCodigo = produtos
                    .ToDictionary(p => p.Codigo, StringComparer.OrdinalIgnoreCase);

                foreach (var par in itensAgrupados)
                {
                    if (!produtosPorCodigo.TryGetValue(par.Key, out var produto))
                        throw new InvalidOperationException($"Produto não encontrado: {par.Key}.");

                    if (produto.Saldo - par.Value < 0)
                        throw new InvalidOperationException($"Saldo insuficiente para o produto {par.Key}.");
                }

                foreach (var par in itensAgrupados)
                {
                    var produto = produtosPorCodigo[par.Key];
                    produto.AbaterEstoque(par.Value);
                    saldosAtualizados.Add(new ItemSaldoDto(produto.Codigo, produto.Saldo));
                }

                await _repositorio.SalvarAlteracoesAsync(ct);
            }, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Resultado<AbaterEstoqueResultadoDto>.Falha(
                ErroAplicacao.Validacao(ex.Message));
        }
        catch (Exception ex) when (ex.GetType().Name == "DbUpdateConcurrencyException")
        {
            return Resultado<AbaterEstoqueResultadoDto>.Falha(
                ErroAplicacao.Conflito("Concorrência detectada. Tente novamente."));
        }

        var resultado = new AbaterEstoqueResultadoDto(saldosAtualizados.AsReadOnly());

        return Resultado<AbaterEstoqueResultadoDto>.Ok(resultado);
    }
}
