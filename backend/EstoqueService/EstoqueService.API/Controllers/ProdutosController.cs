using EstoqueService.Application.DTOs;
using EstoqueService.Application.Interfaces;
using EstoqueService.Application.Resultados;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EstoqueService.API.Controllers
{
    [Route("api/produtos")]
    [ApiController]
    public sealed class ProdutosController : ControllerBase
    {
        private readonly ICriarProdutoUseCase _criar;
        private readonly IAtualizarProdutoUseCase _atualizar;
        private readonly IBuscarProdutoPorIdUseCase _buscarPorId;
        private readonly IListarProdutosUseCase _listar;
        private readonly IAbaterEstoqueUseCase _abater;

        public ProdutosController(
            ICriarProdutoUseCase criar,
            IAtualizarProdutoUseCase atualizar,
            IBuscarProdutoPorIdUseCase buscarPorId,
            IListarProdutosUseCase listar,
            IAbaterEstoqueUseCase abater)
        {
            _criar = criar;
            _atualizar = atualizar;
            _buscarPorId = buscarPorId;
            _listar = listar;
            _abater = abater;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarProdutoEntradaDto entrada, CancellationToken cancellationToken)
        {
            var resultado = await _criar.ExecutarAsync(entrada, cancellationToken);

            if (resultado.Sucesso)
                return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Valor!.Id }, resultado.Valor);

            return ConverterFalha(resultado);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar([FromRoute] Guid id, [FromBody] AtualizarProdutoEntradaDto entrada, CancellationToken cancellationToken)
        {
            var entradaComId = new AtualizarProdutoEntradaDto(id, entrada.Codigo, entrada.Descricao, entrada.Saldo);

            var resultado = await _atualizar.ExecutarAsync(entradaComId, cancellationToken);

            if (resultado.Sucesso)
                return Ok(resultado.Valor);

            return ConverterFalha(resultado);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var resultado = await _buscarPorId.ExecutarAsync(id, cancellationToken);

            if (resultado.Sucesso)
                return Ok(resultado.Valor);

            return ConverterFalha(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> Listar(CancellationToken cancellationToken)
        {
            var resultado = await _listar.ExecutarAsync(cancellationToken);

            if (resultado.Sucesso)
                return Ok(resultado.Valor);

            return ConverterFalha(resultado);
        }

        [HttpPost("abater-estoque")]
        public async Task<IActionResult> AbaterLote([FromBody] AbaterEstoqueEntradaDto entrada, CancellationToken cancellationToken)
        {
            var resultado = await _abater.ExecutarAsync(entrada, cancellationToken);

            if (resultado.Sucesso)
                return Ok(resultado.Valor);

            var erro = resultado.Erros.FirstOrDefault();
            if (erro is not null)
                return BadRequest(new { mensagem = erro.Mensagem });

            return BadRequest();
        }

        private IActionResult ConverterFalha<T>(Resultado<T> resultado)
        {
            var erro = resultado.Erros.FirstOrDefault();

            if (erro is null)
                return BadRequest();

            return erro.Codigo switch
            {
                CodigoErro.NaoEncontrado => NotFound(resultado.Erros),
                CodigoErro.Conflito => Conflict(resultado.Erros),
                _ => BadRequest(resultado.Erros)
            };
        }

    }
}
