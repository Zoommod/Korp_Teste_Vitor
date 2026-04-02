namespace FaturamentoService.Application.DTOs;

public sealed record CriarNotaFiscalEntradaDto(IReadOnlyList<ItemNotaFiscalEntradaDto> Itens);