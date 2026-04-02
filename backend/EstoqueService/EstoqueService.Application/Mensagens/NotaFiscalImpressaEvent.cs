namespace FaturamentoService.Application.Mensagens;

public sealed record NotaFiscalImpressaEvent(Guid IdNotaFiscal, IReadOnlyList<NotaFiscalImpressaEvent.ItemEvento> Itens)
{
    public sealed record ItemEvento(string CodigoProduto, int Quantidade);
}
