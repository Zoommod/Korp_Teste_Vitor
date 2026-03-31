using System;
using EstoqueService.Application.DTOs;
using EstoqueService.Application.Interfaces;
using EstoqueService.Application.Mapeamentos;
using EstoqueService.Application.Resultados;
using EstoqueService.Domain.Entities;
using EstoqueService.Domain.Exceptions;

namespace EstoqueService.Application.CasosDeUso;

public class AtualizarProdutoUseCase : IAtualizarProdutoUseCase
{
    private readonly IProdutoRepository _repositorio;

    public AtualizarProdutoUseCase(IProdutoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Resultado<ProdutoDto>> ExecutarAsync(AtualizarProdutoEntradaDto entrada, CancellationToken cancellationToken)
    {
        if(entrada is null)
            return Resultado<ProdutoDto>.Falha(ErroAplicacao.Validacao("Entrada obrigatória."));
        
        if(entrada.Saldo < 0)
            return Resultado<ProdutoDto>.Falha(ErroAplicacao.Validacao("Saldo não pode ser negativo."));
        
        var produto = await _repositorio.ObterPorIdAsync(entrada.Id, cancellationToken);

        if(produto is null)
            return Resultado<ProdutoDto>.Falha(ErroAplicacao.NaoEncontrado("Produto não encontrado;"));

        try
        {
            produto.AlterarCodigo(entrada.Codigo);
            produto.AlterarDescricao(entrada.Descricao);

            var diferenca = entrada.Saldo - produto.Saldo;
            if(diferenca > 0)
                produto.ReporEstoque(diferenca);
            else if(diferenca < 0)
                produto.AbaterEstoque(-diferenca);
            
            await _repositorio.AtualizarAsync(produto, cancellationToken);

            await _repositorio.SalvarAlteracoesAsync(cancellationToken);

            return Resultado<ProdutoDto>.Ok(ProdutoMapeador.ParaDto(produto));
        }
        catch(ExcecaoDeDominio ex)
        {
            return Resultado<ProdutoDto>.Falha(ErroAplicacao.Validacao(ex.Message));
        }
    }
}
