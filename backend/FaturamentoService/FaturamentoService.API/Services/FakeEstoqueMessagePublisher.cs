using System;
using FaturamentoService.Application.Interfaces;
using FaturamentoService.Domain.Entities;

namespace FaturamentoService.API.Services;

public sealed class FakeEstoqueMessagePublisher : IEstoqueMessagePublisher
{
    public Task PublicarAbatimentosEstoqueAsync(NotaFiscal nota)
    {
        Console.WriteLine($"[FAKE] Abater estoque para nota {nota.Id} com {nota.Itens.Count} itens.");
        return Task.CompletedTask;
    }
}
