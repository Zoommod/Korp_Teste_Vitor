using System;
using EstoqueService.Domain.Exceptions;

namespace EstoqueService.Domain.Entities;

public class Produto
{
    public Guid Id { get; private set; }
    public string Codigo { get; private set; }
    public string Descricao { get; private set; }
    public int Saldo { get; private set; }

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    private Produto() { }

    public Produto(string codigo, string descricao, int saldoInicial)
    {
        AlterarCodigo(codigo);
        AlterarDescricao(descricao);
        DefinirSaldoInicial(saldoInicial);
        Id = Guid.NewGuid();
    }

    public void AlterarCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ExcecaoDeDominio("Código do produto é obrigatório.");

        Codigo = codigo.Trim();
    }

    public void AlterarDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ExcecaoDeDominio("Descrição do produto é obrigatória.");

        Descricao = descricao.Trim();
    }

    public void AbaterEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ExcecaoDeDominio("Quantidade para abatimento deve ser maior que zero.");

        if (Saldo - quantidade < 0)
            throw new ExcecaoDeDominio("Saldo insuficiente para abatimento.");

        Saldo -= quantidade;
    }

    public void ReporEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ExcecaoDeDominio("Quantidade para reposição deve ser maior que zero.");

        Saldo += quantidade;
    }

    private void DefinirSaldoInicial(int saldoInicial)
    {
        if (saldoInicial < 0)
            throw new ExcecaoDeDominio("Saldo inicial não pode ser negativo");

        Saldo = saldoInicial;
    }
}
