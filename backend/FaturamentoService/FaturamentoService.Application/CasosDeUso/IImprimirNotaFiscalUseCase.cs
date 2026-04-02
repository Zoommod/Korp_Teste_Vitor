using System;
using FaturamentoService.Application.DTOs;
using FaturamentoService.Application.Resultados;

namespace FaturamentoService.Application.CasosDeUso;

public interface IImprimirNotaFiscalUseCase
{
    Task<Resultado<NotaFiscalSaidaDto>> ExecutarAsync(Guid id);
}
