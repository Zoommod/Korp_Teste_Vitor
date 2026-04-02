namespace FaturamentoService.Application.DTOs;

public sealed record NotaFiscalSaidaDto(Guid Id, int Numero, string Status, IReadOnlyList<ItemNotaFiscalEntradaDto> Itens);
