using System;
using FaturamentoService.Domain.Entities;

namespace FaturamentoService.Application.Interfaces;

public interface INotaFiscalRepository
{
    Task AdicionarAsync(NotaFiscal nota);
    Task<NotaFiscal?> ObterPorIdAsync(Guid id);
    Task AtualizarAsync(NotaFiscal nota);
}
