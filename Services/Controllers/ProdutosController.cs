using Business.Interface;
using Domain.Constantes;
using Domain.DTO;
using Domain.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace Services.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoBusiness _produtoBusiness;
        public ProdutosController(IProdutoBusiness produtoBusiness)
        {
            _produtoBusiness = produtoBusiness;
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Recupera(Guid? id)
        {
            try
            {
                var produto = _produtoBusiness.Recuperar(id);
                return Ok(produto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{Constantes.MensagensErro.ERRO_500}\n{ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult Listar(int? maximo)
        {
            try
            {
                var produtos = _produtoBusiness.Listar(maximo);
                return Ok(produtos);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{Constantes.MensagensErro.ERRO_500}\n{ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Criar(ProdutoDTO produtoDTO)
        {
            try
            {
                var codigo = _produtoBusiness.Criar(produtoDTO);
                return StatusCode(StatusCodes.Status201Created, $"{Constantes.MensagensSucesso.PRODUTO_CRIADO}{codigo}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{Constantes.MensagensErro.ERRO_500}\n{ex.Message}");
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Atualizar(Guid? id, [FromBody]ProdutoDTO produto)
        {
            try
            {
                _produtoBusiness.Atualizar(id, produto);
                return Ok(Constantes.MensagensSucesso.PRODUTO_ATUALIZADO);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{Constantes.MensagensErro.ERRO_500}\n{ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Deletar(Guid? id)
        {
            try
            {
                _produtoBusiness.Deletar(id);
                return Ok(Constantes.MensagensSucesso.PRODUTO_DELETADO);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{Constantes.MensagensErro.ERRO_500}\n{ex.Message}");
            }
        }

        [HttpPost]
        [Route("sincronizacao")]
        public IActionResult Sincronizar()
        {
            try
            {
                _produtoBusiness.SincronizarBases();
                return Ok(Constantes.MensagensSucesso.PRODUTOS_SINCRONIZADOS);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{Constantes.MensagensErro.ERRO_500}\n{ex.Message}");
            }
        }
    }
}
