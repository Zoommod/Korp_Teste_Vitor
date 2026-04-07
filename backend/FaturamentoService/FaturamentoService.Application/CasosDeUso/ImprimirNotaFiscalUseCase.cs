using System;
using FaturamentoService.Application.DTOs;
using FaturamentoService.Application.Interfaces;
using FaturamentoService.Application.Resultados;
using FaturamentoService.Domain.Exceptions;

namespace FaturamentoService.Application.CasosDeUso;

public class ImprimirNotaFiscalUseCase : IImprimirNotaFiscalUseCase
{
    private readonly INotaFiscalRepository _repository;
    private readonly IEstoqueClient _estoqueClient;

    public ImprimirNotaFiscalUseCase(
        INotaFiscalRepository repository,
        IEstoqueClient estoqueClient)
    {
        _repository = repository;
        _estoqueClient = estoqueClient;
    }

    public async Task<Resultado<NotaFiscalSaidaDto>> ExecutarAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        const string MensagemServicoIndisponivel =
            "Servico de estoque temporariamente indisponivel. A nota nao pode ser impressa.";

        var nota = await _repository.ObterPorIdAsync(id, cancellationToken);

        if (nota is null)
            return Resultado<NotaFiscalSaidaDto>.Falha(
                ErroAplicacao.NaoEncontrado("Nota fiscal nao encontrada."));

        var resultadoEstoque = await _estoqueClient.AbaterEstoqueAsync(nota, cancellationToken);
        if (!resultadoEstoque.Sucesso)
        {
            var mensagem = resultadoEstoque.MensagemErro ?? "Erro ao abater estoque.";

            if (string.Equals(mensagem, MensagemServicoIndisponivel, StringComparison.OrdinalIgnoreCase))
            {
                return Resultado<NotaFiscalSaidaDto>.Falha(
                    ErroAplicacao.ServicoIndisponivel(mensagem));
            }

            return Resultado<NotaFiscalSaidaDto>.Falha(
                ErroAplicacao.Validacao(mensagem));
        }

        try
        {
            nota.Imprimir();
        }
        catch (ExcecaoDeDominio ex)
        {
            return Resultado<NotaFiscalSaidaDto>.Falha(
                ErroAplicacao.Validacao(ex.Message));
        }

        await _repository.AtualizarAsync(nota, cancellationToken);

        var itens = nota.Itens
            .Select(i => new ItemNotaFiscalEntradaDto(i.CodigoProduto, i.Quantidade))
            .ToList()
            .AsReadOnly();

        var saida = new NotaFiscalSaidaDto(
            nota.Id,
            nota.Numero,
            nota.Status.ToString(),
            itens
        );

        return Resultado<NotaFiscalSaidaDto>.Ok(saida);
    }
}
