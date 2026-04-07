using System.Data;
using EstoqueService.Application.Interfaces;
using EstoqueService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public sealed class TransacaoEstoque : ITransacaoEstoque
{
    private readonly EstoqueDbContext _context;

    public TransacaoEstoque(EstoqueDbContext context)
    {
        _context = context;
    }

    public async Task ExecutarAsync(
        Func<CancellationToken, Task> operacao,
        CancellationToken cancellationToken)
    {
        await using var transacao =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);

        await operacao(cancellationToken);

        await transacao.CommitAsync(cancellationToken);
    }
}
