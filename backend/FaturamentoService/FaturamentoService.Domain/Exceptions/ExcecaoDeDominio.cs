using System;

namespace FaturamentoService.Domain.Exceptions;

public sealed class ExcecaoDeDominio : Exception
{
    public ExcecaoDeDominio(string mensagem) : base(mensagem)
    {
        
    }
}
