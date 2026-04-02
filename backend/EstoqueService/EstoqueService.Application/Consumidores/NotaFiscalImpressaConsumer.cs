using System;
using EstoqueService.Application.Interfaces;
using FaturamentoService.Application.Mensagens;
using MassTransit;

namespace EstoqueService.Application.Consumidores;

public class NotaFiscalImpressaConsumer : IConsumer<NotaFiscalImpressaEvent>
{
    private readonly IProdutoRepository _repository;

    public NotaFiscalImpressaConsumer(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<NotaFiscalImpressaEvent> context)
    {
        var mensagem = context.Message;

        Console.WriteLine($"[NotaFiscalImpressaConsumer]Inicio. IdNotaFiscal={mensagem.IdNotaFiscal}");

        foreach(var item in mensagem.Itens)
        {
            var produto = await _repository.ObterPorCodigoAsync(item.CodigoProduto, context.CancellationToken);

            if(produto is null)
                throw new InvalidOperationException($"Produto não encontrado: {item.CodigoProduto}");
            
            produto.AbaterEstoque(item.Quantidade);
            await _repository.AtualizarAsync(produto, context.CancellationToken);

        }

        await _repository.SalvarAlteracoesAsync(context.CancellationToken);

        Console.WriteLine($"[NotaFiscalImpressaConsumer] Fim. IdNotaFiscal={mensagem.IdNotaFiscal}");
    }
}
