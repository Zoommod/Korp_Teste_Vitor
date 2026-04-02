namespace FaturamentoService.Application.Mensagens;

public record class NotaFiscalImpressaEvent(Guid IdNotaFiscal, IReadOnlyList<NotaFiscalImpressaEvent.ItemEvento> Itens)
{
    public sealed record ItemEvento(string CodigoProduto, int Quantidade);
}
