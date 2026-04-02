using System;
using FaturamentoService.Domain.Exceptions;

namespace FaturamentoService.Domain.Entities;

public sealed class ItemNotaFiscal
{
    public Guid Id { get; private set; }
    public string CodigoProduto { get; private set; }
    public int Quantidade { get; private set; }

    private ItemNotaFiscal(){}

    public ItemNotaFiscal(string codigoProduto, int quantidade)
    {
        if(string.IsNullOrWhiteSpace(codigoProduto))
            throw new ExcecaoDeDominio("Código do produto é obrigatório.");
        
        if(quantidade <= 0)
            throw new ExcecaoDeDominio("Quantidade deve ser maior que zero.");
        
        Id = Guid.NewGuid();
        CodigoProduto = codigoProduto.Trim();
        Quantidade = quantidade;
    }
}
