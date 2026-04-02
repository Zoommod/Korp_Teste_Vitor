using System;
using System.Collections.Generic;

namespace FaturamentoService.Application.Resultados;

public enum CodigoErro
{
    Validacao,
    NaoEncontrado,
    Conflito
}

public sealed class ErroAplicacao
{
    public CodigoErro Codigo { get; }
    public string Mensagem { get; }

    private ErroAplicacao(CodigoErro codigo, string mensagem)
    {
        Codigo = codigo;
        Mensagem = mensagem;
    }

    public static ErroAplicacao Validacao(string mensagem) => new(CodigoErro.Validacao, mensagem);
    public static ErroAplicacao NaoEncontrado(string mensagem) => new(CodigoErro.NaoEncontrado, mensagem);
    public static ErroAplicacao Conflito(string mensagem) => new(CodigoErro.Conflito, mensagem);
}

public sealed class Resultado<T>
{
    public bool Sucesso { get; }
    public T? Valor { get; }
    public IReadOnlyList<ErroAplicacao> Erros { get; }

    private Resultado(bool sucesso, T? valor, IReadOnlyList<ErroAplicacao> erros)
    {
        Sucesso = sucesso;
        Valor = valor;
        Erros = erros;
    }

    public static Resultado<T> Ok(T valor) => new(true, valor, Array.Empty<ErroAplicacao>());

    public static Resultado<T> Falha(params ErroAplicacao[] erros) => new(false, default, erros);

    public static Resultado<T> Falha(string mensagem) => new(false, default, new[] { ErroAplicacao.Validacao(mensagem) });
}