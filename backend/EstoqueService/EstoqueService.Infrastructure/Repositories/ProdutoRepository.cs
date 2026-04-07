using System;
using System.Data.Common;
using EstoqueService.Domain.Entities;
using EstoqueService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EstoqueService.Application.Interfaces;

namespace EstoqueService.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly EstoqueDbContext _context;

    public ProdutoRepository(EstoqueDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Produto produto, CancellationToken cancellationToken)
    {
        await _context.Produtos.AddAsync(produto, cancellationToken);
    }

    public Task AtualizarAsync(Produto produto, CancellationToken cancellationToken)
    {
        _context.Produtos.Update(produto);
        return Task.CompletedTask;
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Produto?> ObterPorCodigoAsync(string codigo, CancellationToken cancellationToken)
    {
        return await _context.Produtos.FirstOrDefaultAsync(p => p.Codigo == codigo, cancellationToken);
    }

    public async Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken cancellationToken)
    {
        return await _context.Produtos.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task SalvarAlteracoesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Produto>> ObterPorCodigosAsync(IReadOnlyCollection<string> codigos, CancellationToken cancellationToken)
    {
        return await _context.Produtos
            .Where(p => codigos.Contains(p.Codigo))
            .ToListAsync(cancellationToken);
    }
}
