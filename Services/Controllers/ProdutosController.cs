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
        public IActionResult Recupera(int? id)
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
                _produtoBusiness.Criar(produtoDTO);
                return Ok(Constantes.MensagensSucesso.PRODUTO_CRIADO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{Constantes.MensagensErro.ERRO_500}\n{ex.Message}");
            }
        }
    }
}
