using System;
using FaturamentoService.Application.DTOs;
using FaturamentoService.Application.Interfaces;
using FaturamentoService.Application.Resultados;
using FaturamentoService.Domain.Entities;
using FaturamentoService.Domain.Exceptions;

namespace FaturamentoService.Application.CasosDeUso;

public sealed class CriarNotaFiscalUseCase : ICriarNotaFiscalUseCase
{
    private readonly INotaFiscalRepository _repository;

    public CriarNotaFiscalUseCase(INotaFiscalRepository repository)
    {
        _repository = repository;
    }

    public async Task<Resultado<Guid>> ExecutarAsync(
        CriarNotaFiscalEntradaDto entrada,
        CancellationToken cancellationToken)
    {
        if (entrada?.Itens is null || entrada.Itens.Count == 0)
            return Resultado<Guid>.Falha("Lista de itens obrigatória.");

        var nota = new NotaFiscal();

        try
        {
            foreach (var item in entrada.Itens)
                nota.AdicionarItem(item.CodigoProduto, item.Quantidade);
        }
        catch (ExcecaoDeDominio ex)
        {
            return Resultado<Guid>.Falha(ex.Message);
        }

        await _repository.AdicionarAsync(nota, cancellationToken);

        return Resultado<Guid>.Ok(nota.Id);
    }
}
