using FaturamentoService.Application.CasosDeUso;
using FaturamentoService.Application.DTOs;
using FaturamentoService.Application.Resultados;
using FaturamentoService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FaturamentoService.API.API
{
    [Route("api/notas-fiscais")]
    [ApiController]
    public sealed class NotasFiscaisController : ControllerBase
    {
        private readonly ICriarNotaFiscalUseCase _criar;
        private readonly IImprimirNotaFiscalUseCase _imprimir;

        public NotasFiscaisController(
            ICriarNotaFiscalUseCase criar,
            IImprimirNotaFiscalUseCase imprimir)
        {
            _criar = criar;
            _imprimir = imprimir;
        }

        [HttpPost]
        public async Task<IActionResult> Criar(
            [FromBody] CriarNotaFiscalEntradaDto entrada,
            CancellationToken cancellationToken)
        {
            var resultado = await _criar.ExecutarAsync(entrada, cancellationToken);

            if (resultado.Sucesso)
                return StatusCode(StatusCodes.Status201Created, resultado.Valor);

            return ConverterFalhaCriacao(resultado);
        }

        [HttpPost("{id:guid}/imprimir")]
        public async Task<IActionResult> Imprimir(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var resultado = await _imprimir.ExecutarAsync(id, cancellationToken);

            if (resultado.Sucesso)
                return Ok(resultado.Valor);

            return ConverterFalhaImpressao(resultado);
        }

        private IActionResult ConverterFalhaCriacao<T>(Resultado<T> resultado)
        {
            var erro = resultado.Erros.FirstOrDefault();

            if (erro is null)
                return BadRequest();

            return erro.Codigo switch
            {
                CodigoErro.Validacao => BadRequest(resultado.Erros),
                CodigoErro.Conflito => BadRequest(resultado.Erros),
                CodigoErro.ServicoIndisponivel => StatusCode(StatusCodes.Status503ServiceUnavailable, resultado.Erros),
                _ => BadRequest(resultado.Erros)
            };
        }

        private IActionResult ConverterFalhaImpressao<T>(Resultado<T> resultado)
        {
            var erro = resultado.Erros.FirstOrDefault();

            if (erro is null)
                return BadRequest();

            return erro.Codigo switch
            {
                CodigoErro.NaoEncontrado => NotFound(resultado.Erros),
                CodigoErro.Validacao => BadRequest(resultado.Erros),
                CodigoErro.ServicoIndisponivel => StatusCode(StatusCodes.Status503ServiceUnavailable, resultado.Erros),
                _ => BadRequest(resultado.Erros)
            };
        }
    }
}
