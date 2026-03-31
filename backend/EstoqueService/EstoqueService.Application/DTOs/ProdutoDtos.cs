using System;
using System.Collections.Specialized;
using System.Net.ServerSentEvents;

namespace EstoqueService.Application.DTOs;

public sealed record CriarProdutoEntradaDto(string Codigo, string Descricao, int SaldoInicial);
public sealed record AtualizarProdutoEntradaDto(Guid Id, string Codigo, string Descricao, int Saldo);

public sealed record ProdutoDto(Guid Id, string Codigo, string Descricao, int Saldo);

public sealed record AbaterEstoqueEntradaDto(IReadOnlyList<ItemAbateEstoqueDto> Itens);
public sealed record ItemAbateEstoqueDto(string CodigoProduto, int Quantidade);

public sealed record AbaterEstoqueResultadoDto(IReadOnlyList<ItemSaldoDto> Itens);
public sealed record ItemSaldoDto(string CodigoProduto, int SaldoAtual);