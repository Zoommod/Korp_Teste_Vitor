using System;
using FaturamentoService.Domain.Entities;

namespace FaturamentoService.Application.Interfaces;

public interface IEstoqueClient
{
    Task<(bool Sucesso, string? MensagemErro)> AbaterEstoqueAsync(NotaFiscal nota, CancellationToken cancellationToken);
}
