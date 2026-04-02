using System;
using FaturamentoService.Domain.Entities;

namespace FaturamentoService.Application.Interfaces;

public interface IEstoqueMessagePublisher
{
    Task PublicarAbatimentosEstoqueAsync(NotaFiscal nota);
}
