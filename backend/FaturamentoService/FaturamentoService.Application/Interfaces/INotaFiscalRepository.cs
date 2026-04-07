using System;
using FaturamentoService.Domain.Entities;

namespace FaturamentoService.Application.Interfaces;

public interface INotaFiscalRepository
{
    Task AdicionarAsync(NotaFiscal nota, CancellationToken cancellationToken);
    Task<NotaFiscal?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task AtualizarAsync(NotaFiscal nota, CancellationToken cancellationToken);
}
