using System;
using EstoqueService.Domain.Entities;

namespace EstoqueService.Application.Interfaces;

public interface IProdutoRepository
{
    Task AdicionarAsync(Produto produto, CancellationToken cancellationToken);
    Task AtualizarAsync(Produto produto, CancellationToken cancellationToken);
    Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Produto?> ObterPorCodigoAsync(string codigo, CancellationToken cancellationToken);
    Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken cancellationToken);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken);
}
