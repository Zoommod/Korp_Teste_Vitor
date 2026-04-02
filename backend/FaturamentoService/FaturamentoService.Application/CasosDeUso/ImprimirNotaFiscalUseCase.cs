using System;
using FaturamentoService.Application.DTOs;
using FaturamentoService.Application.Interfaces;
using FaturamentoService.Application.Resultados;
using FaturamentoService.Domain.Exceptions;

namespace FaturamentoService.Application.CasosDeUso;

public class ImprimirNotaFiscalUseCase : IImprimirNotaFiscalUseCase
{
    private readonly INotaFiscalRepository _repository;
    private readonly IEstoqueMessagePublisher _estoquePublisher;

    public ImprimirNotaFiscalUseCase(INotaFiscalRepository repository, IEstoqueMessagePublisher estoquePublisher)
    {
        _repository = repository;
        _estoquePublisher = estoquePublisher;
    }

    public async Task<Resultado<NotaFiscalSaidaDto>> ExecutarAsync(Guid id)
    {
        var nota = await _repository.ObterPorIdAsync(id);

        if(nota is null)
            return Resultado<NotaFiscalSaidaDto>.Falha("Nota fiscal não encontrada.");

        try
        {
            nota.Imprimir();
        }
        catch(ExcecaoDeDominio ex)
        {
            return Resultado<NotaFiscalSaidaDto>.Falha(ex.Message);
        }

        await _repository.AtualizarAsync(nota);
        await _estoquePublisher.PublicarAbatimentosEstoqueAsync(nota);

        var itens = nota.Itens
            .Select(i => new ItemNotaFiscalEntradaDto(i.CodigoProduto, i.Quantidade))
            .ToList()
            .AsReadOnly();
        
        var saida = new NotaFiscalSaidaDto(
            nota.Id,
            nota.Numero,
            nota.Status.ToString(),
            itens
        );

        return Resultado<NotaFiscalSaidaDto>.Ok(saida);
    }
}
