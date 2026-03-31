using System;

namespace EstoqueService.Domain.Exceptions;

public class ExcecaoDeDominio : Exception
{
    public ExcecaoDeDominio(string mensagem) : base(mensagem)
    {

    }
}
