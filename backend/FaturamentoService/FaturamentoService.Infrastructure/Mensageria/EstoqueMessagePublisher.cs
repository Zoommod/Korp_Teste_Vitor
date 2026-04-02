using System;
using FaturamentoService.Application.Interfaces;
using FaturamentoService.Application.Mensagens;
using FaturamentoService.Domain.Entities;
using MassTransit;

namespace FaturamentoService.Infrastructure.Mensageria;

public sealed class EstoqueMessagePublisher : IEstoqueMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EstoqueMessagePublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublicarAbatimentosEstoqueAsync(NotaFiscal nota)
    {
        var itens = nota.Itens
            .Select(i => new NotaFiscalImpressaEvent.ItemEvento(i.CodigoProduto, i.Quantidade))
            .ToList()
            .AsReadOnly();

        var evento = new NotaFiscalImpressaEvent(nota.Id, itens);

        return _publishEndpoint.Publish(evento);
    }
}
