using System;

namespace EstoqueService.Application.Interfaces;

public interface ITransacaoEstoque
{
    Task ExecutarAsync(Func<CancellationToken, Task> operacao, CancellationToken cancellationToken);
}
