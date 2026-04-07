using System;
using FaturamentoService.Application.Interfaces;
using FaturamentoService.Domain.Entities;
using FaturamentoService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FaturamentoService.Infrastructure.Repositories;

public sealed class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly FaturamentoDbContext _context;

    public NotaFiscalRepository(FaturamentoDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(NotaFiscal nota, CancellationToken cancellationToken)
    {
        await _context.NotaFiscais.AddAsync(nota, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<NotaFiscal?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.NotaFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task AtualizarAsync(NotaFiscal nota, CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
