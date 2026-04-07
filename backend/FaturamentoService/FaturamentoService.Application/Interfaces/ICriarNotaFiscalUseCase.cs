using System;
using FaturamentoService.Application.DTOs;
using FaturamentoService.Application.Resultados;

namespace FaturamentoService.Application.Interfaces;

public interface ICriarNotaFiscalUseCase
{
    Task<Resultado<Guid>> ExecutarAsync(CriarNotaFiscalEntradaDto entrada,CancellationToken cancellationToken);
}
