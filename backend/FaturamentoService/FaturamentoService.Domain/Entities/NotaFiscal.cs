using System;
using FaturamentoService.Domain.Enums;
using FaturamentoService.Domain.Exceptions;

namespace FaturamentoService.Domain.Entities;

public sealed class NotaFiscal
{
    private readonly List<ItemNotaFiscal> _itens = new();

    public Guid Id { get; private set; }
    public int Numero { get; private set; }
    public StatusNotaFiscal Status { get; private set; }

    public IReadOnlyCollection<ItemNotaFiscal> Itens => _itens.AsReadOnly();

    public NotaFiscal()
    {
        Id = Guid.NewGuid();
        Status = StatusNotaFiscal.Aberta;
    }

    public void AdicionarItem(string codigoProduto, int quantidade)
    {
        var item = new ItemNotaFiscal(codigoProduto, quantidade);
        _itens.Add(item);
    }

    public void Imprimir()
    {
        if(Status != StatusNotaFiscal.Aberta)
            throw new ExcecaoDeDominio("A nota fiscal precisa estar aberta para ser impressa");

        Status = StatusNotaFiscal.Fechada;
    }
}
