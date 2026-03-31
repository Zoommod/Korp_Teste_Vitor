using System;
using EstoqueService.Application.DTOs;
using EstoqueService.Domain.Entities;

namespace EstoqueService.Application.Mapeamentos;

public static class ProdutoMapeador
{
    public static ProdutoDto ParaDto(Produto produto) => new(produto.Id, produto.Codigo, produto.Descricao, produto.Saldo);
}
