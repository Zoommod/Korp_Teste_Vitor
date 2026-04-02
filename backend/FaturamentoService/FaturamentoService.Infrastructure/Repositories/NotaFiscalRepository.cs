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

    public async Task AdicionarAsync(NotaFiscal nota)
    {
        await _context.NotaFiscais.AddAsync(nota);
        await _context.SaveChangesAsync();
    }

    public async Task<NotaFiscal?> ObterPorIdAsync(Guid id)
    {
        return await _context.NotaFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task AtualizarAsync(NotaFiscal nota)
    {
        await _context.SaveChangesAsync();
    }
}
