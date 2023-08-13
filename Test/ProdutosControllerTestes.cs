using AutoMapper;
using Business.Interface;
using Business.Mapper;
using Data.Repositories.Relacional.Interface;
using Domain.Constantes;
using Domain.DTO;
using Domain.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Controllers;
using System.ComponentModel.DataAnnotations;

namespace Test
{
    [TestClass]
    public class ProdutosControllerTestes
    {
        #region Validação objeto

        [TestMethod]
        public void ProdutosController_ValidarPreco_PrecoCorreto()
        {
            //Arrange
            ProdutoDTO produto = new()
            {
                Nome = "Caneta",
                Preco = 5,
                DataCriacao = DateTime.Now,
                Quantidade = 10,
                ValorTotal = 5 * 10
            };

            //Act
            var erros = ValidateObject(produto);

            //Assert
            Assert.AreEqual(0, erros.Count());
        }

        [TestMethod]
        public void ProdutosController_ValidarPreco_PrecoNegativo()
        {
            //Arrange
            ProdutoDTO produto = new()
            {
                Nome = "Caneta",
                Preco = -5,
                DataCriacao = DateTime.Now,
                Quantidade = 10,
                ValorTotal = 5 * 10
            };

            //Act
            var erros = ValidateObject(produto);

            //Assert
            Assert.AreEqual(1, erros.Count());
            Assert.AreEqual(erros[0].ErrorMessage, "O valor enviado no campo Preço é inválido.");
        }

        [TestMethod]
        public void ProdutosController_ValidarObjeto_ProdutoSemNome()
        {
            //Arrange
            ProdutoDTO produto = new()
            {
                Preco = 5,
                DataCriacao = DateTime.Now,
                Quantidade = 10,
                ValorTotal = 5 * 10
            };

            //Act
            var erros = ValidateObject(produto);

            //Assert
            Assert.AreEqual(1, erros.Count());
            Assert.AreEqual(erros[0].ErrorMessage, "O campo Nome é de preenchimento obrigatório.");
        }

        [TestMethod]
        public void ProdutosController_ValidarObjeto_ProdutoSemPreco()
        {
            //Arrange
            ProdutoDTO produto = new()
            {
                Nome = "Caneta",
                DataCriacao = DateTime.Now,
                Quantidade = 10,
                ValorTotal = 5 * 10
            };

            //Act
            var erros = ValidateObject(produto);

            //Assert
            Assert.AreEqual(1, erros.Count());
            Assert.AreEqual(erros[0].ErrorMessage, "O campo Preço é de preenchimento obrigatório.");
        }

        [TestMethod]
        public void ProdutosController_ValidarObjeto_ProdutoSemQuantidade()
        {
            //Arrange
            ProdutoDTO produto = new()
            {
                Nome = "Caneta",
                Preco = 5,
                DataCriacao = DateTime.Now,
                ValorTotal = 5 * 10
            };

            //Act
            var erros = ValidateObject(produto);

            //Assert
            Assert.AreEqual(1, erros.Count());
            Assert.AreEqual(erros[0].ErrorMessage, "O campo Quantidade é de preenchimento obrigatório.");
        }

        [TestMethod]
        public void ProdutosController_ValidarObjeto_ProdutoDataCriacao()
        {
            //Arrange
            ProdutoDTO produto = new()
            {
                Nome = "Caneta",
                Preco = 5,
                Quantidade = 10,
                ValorTotal = 5 * 10
            };

            //Act
            var erros = ValidateObject(produto);

            //Assert
            Assert.AreEqual(1, erros.Count());
            Assert.AreEqual(erros[0].ErrorMessage, "O campo Data de Criação é de preenchimento obrigatório.");
        }

        [TestMethod]
        public void ProdutosController_ValidarObjeto_CampoCalculadoValorTotal()
        {
            //Arrange
            Produto produto = new()
            {
                Nome = "Caneta",
                Preco = 5,
                DataCriacao = DateTime.Now,
                Quantidade = 10
            };

            var config = new MapperConfiguration(cfg => { cfg.AddProfile<MobilusMapper>(); });
            IMapper _mapper = config.CreateMapper();

            //Act
            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            //Assert
            Assert.AreEqual(produtoDTO.ValorTotal, produto.Preco * produto.Quantidade);
        }


        private static IList<ValidationResult> ValidateObject(ProdutoDTO produtoDTO)
        {
            var validate = new List<ValidationResult>();
            var context = new ValidationContext(produtoDTO, null, null);
            Validator.TryValidateObject(produtoDTO, context, validate, true);
            return validate;
        }

        #endregion

        #region Controller ações

        [TestMethod]
        public void ProdutosController_Recupera_ProdutoEncontrado()
        {
            //Arrange
            ProdutoDTO produto = new()
            {
                Nome = "Caneta",
                Preco = 5,
                DataCriacao = DateTime.Now,
                Quantidade = 10,
                ValorTotal = 5 * 10
            };
            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Recuperar(It.IsAny<int>())).Returns(produto);
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Recupera(It.IsAny<int?>()) as OkObjectResult;

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.StatusCode, 200);
        }

        [TestMethod]
        public void ProdutosController_Recupera_ProdutoNaoEncontrado()
        {
            //Arrange
            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Recuperar(It.IsAny<int>())).Throws(new KeyNotFoundException(Constantes.MensagensErro.PRODUTO_NAO_ENCONTRADO));
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Recupera(4);

            //Assert
            Assert.IsInstanceOfType(resultado, typeof(ObjectResult));
            var objectResult = resultado as ObjectResult;
            Assert.AreEqual(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.AreEqual(Constantes.MensagensErro.PRODUTO_NAO_ENCONTRADO, objectResult.Value.ToString());
        }

        [TestMethod]
        public void ProdutosController_Recupera_ErroInterno()
        {
            //Arrange
            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Recuperar(It.IsAny<int>())).Throws(new Exception(Constantes.MensagensErro.ERRO_500));
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Recupera(4);

            //Assert
            Assert.IsInstanceOfType(resultado, typeof(ObjectResult));
            var objectResult = resultado as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [TestMethod]
        public void ProdutosController_Listar_ListaRecuperada()
        {
            //Arrange
            List<ProdutoDTO> produtos = new()
            {
                new()
                {
                    Nome = "Caneta",
                    Preco = 5,
                    DataCriacao = DateTime.Now,
                    Quantidade = 10,
                    ValorTotal = 5 * 10
                },

                new()
                {
                    Nome = "Lápis",
                    Preco = 3,
                    DataCriacao = DateTime.Now,
                    Quantidade = 8,
                    ValorTotal = 3 * 8
                },

                new()
                {
                    Nome = "Borracha",
                    Preco = 2,
                    DataCriacao = DateTime.Now,
                    Quantidade = 8,
                    ValorTotal = 2 * 8
                }
            };
            
            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Listar(null)).Returns(produtos);
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Listar(null) as OkObjectResult;

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.StatusCode, 200);
            Assert.AreEqual(3, (resultado.Value as List<ProdutoDTO>).Count);
        }

        [TestMethod]
        public void ProdutosController_Listar_ListaRecuperadaComParametroMaximo()
        {
            //Arrange
            List<ProdutoDTO> produtos = new()
            {
                new()
                {
                    Nome = "Caneta",
                    Preco = 5,
                    DataCriacao = DateTime.Now,
                    Quantidade = 10,
                    ValorTotal = 5 * 10
                },

                new()
                {
                    Nome = "Lápis",
                    Preco = 3,
                    DataCriacao = DateTime.Now,
                    Quantidade = 8,
                    ValorTotal = 3 * 8
                },

                new()
                {
                    Nome = "Borracha",
                    Preco = 2,
                    DataCriacao = DateTime.Now,
                    Quantidade = 8,
                    ValorTotal = 2 * 8
                }
            };
            int maximo = 1;

            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Listar(maximo)).Returns(produtos.Take(maximo).ToList());
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Listar(maximo) as OkObjectResult;

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.StatusCode, 200);
            Assert.AreEqual(1, (resultado.Value as List<ProdutoDTO>).Count);
        }

        [TestMethod]
        public void ProdutosController_Listar_ProdutosNaoEncontrado()
        {
            //Arrange
            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Listar(null)).Throws(new KeyNotFoundException(Constantes.MensagensErro.PRODUTO_NAO_ENCONTRADO));
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Listar(null);

            //Assert
            Assert.IsInstanceOfType(resultado, typeof(ObjectResult));
            var objectResult = resultado as ObjectResult;
            Assert.AreEqual(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.AreEqual(Constantes.MensagensErro.PRODUTO_NAO_ENCONTRADO, objectResult.Value.ToString());
        }

        [TestMethod]
        public void ProdutosController_Listar_ErroInterno()
        {
            //Arrange
            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Listar(null)).Throws(new Exception(Constantes.MensagensErro.ERRO_500));
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Listar(null);

            //Assert
            Assert.IsInstanceOfType(resultado, typeof(ObjectResult));
            var objectResult = resultado as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [TestMethod]
        public void ProdutosController_Criar_ProdutoCriado()
        {
            //Arrange
            ProdutoDTO produto = new()
            {
                Nome = "Caneta",
                Preco = 5,
                DataCriacao = DateTime.Now,
                Quantidade = 10,
                ValorTotal = 5 * 10
            };

            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Criar(produto));
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Criar(produto) as ObjectResult;

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.StatusCode, 201);
            Assert.AreEqual(Constantes.MensagensSucesso.PRODUTO_CRIADO, resultado.Value.ToString());
        }

        [TestMethod]
        public void ProdutosController_Criar_ErroInterno()
        {
            //Arrange
            ProdutoDTO produto = new()
            {
                Nome = "Caneta",
                Preco = 5,
                DataCriacao = DateTime.Now,
                Quantidade = 10,
                ValorTotal = 5 * 10
            };

            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Criar(produto)).Throws(new Exception(Constantes.MensagensErro.ERRO_500));
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Criar(produto) as ObjectResult;

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.StatusCode, 500);
        }

        [TestMethod]
        public void ProdutosController_Atualizar_ProdutoAtualizado()
        {
            //Arrange
            ProdutoDTO produto = new()
            {
                Id = 1,
                Nome = "Caneta",
                Preco = 5,
                DataCriacao = DateTime.Now,
                Quantidade = 10,
                ValorTotal = 5 * 10,
                IdCompartilhado = 1
            };

            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Atualizar(produto.IdCompartilhado, produto));
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Atualizar(produto.IdCompartilhado, produto) as ObjectResult;

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.StatusCode, 200);
            Assert.AreEqual(Constantes.MensagensSucesso.PRODUTO_ATUALIZADO, resultado.Value.ToString());
        }

        [TestMethod]
        public void ProdutosController_Atualizar_ProdutoBancoNaoEncontrado()
        {
            //Arrange
            ProdutoDTO produto = new()
            {
                Id = 1,
                Nome = "Caneta",
                Preco = 5,
                DataCriacao = DateTime.Now,
                Quantidade = 10,
                ValorTotal = 5 * 10,
                IdCompartilhado = 1
            };

            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Atualizar(2894, produto)).Throws(new KeyNotFoundException(Constantes.MensagensErro.PRODUTO_NAO_ENCONTRADO));
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Atualizar(2894, produto) as ObjectResult;

            //Assert
            Assert.AreEqual(StatusCodes.Status404NotFound, resultado.StatusCode);
            Assert.AreEqual(Constantes.MensagensErro.PRODUTO_NAO_ENCONTRADO, resultado.Value.ToString());
        }

        [TestMethod]
        public void ProdutosController_Atualizar_ErroInterno()
        {
            //Arrange
            ProdutoDTO produto = new()
            {
                Id = 1,
                Nome = "Caneta",
                Preco = 5,
                DataCriacao = DateTime.Now,
                Quantidade = 10,
                ValorTotal = 5 * 10,
                IdCompartilhado = 1
            };

            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Atualizar(2894, produto)).Throws(new Exception(Constantes.MensagensErro.ERRO_500));
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Atualizar(2894, produto) as ObjectResult;

            //Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, resultado.StatusCode);
        }

        [TestMethod]
        public void ProdutosController_Deletar_ProdutoDeletado()
        {
            //Arrange
            int idProduto = 1; 

            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Deletar(idProduto));
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Deletar(idProduto) as ObjectResult;

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado.StatusCode, 200);
            Assert.AreEqual(Constantes.MensagensSucesso.PRODUTO_DELETADO, resultado.Value.ToString());
        }

        [TestMethod]
        public void ProdutosController_Deletar_ProdutoBancoNaoEncontrado()
        {
            //Arrange
            int idProduto = 1;

            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Deletar(idProduto)).Throws(new KeyNotFoundException(Constantes.MensagensErro.PRODUTO_NAO_ENCONTRADO));
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Deletar(idProduto) as ObjectResult;

            //Assert
            Assert.AreEqual(StatusCodes.Status404NotFound, resultado.StatusCode);
            Assert.AreEqual(Constantes.MensagensErro.PRODUTO_NAO_ENCONTRADO, resultado.Value.ToString());
        }

        [TestMethod]
        public void ProdutosController_Deletar_ErroInterno()
        {
            //Arrange
            int idProduto = 1;

            var produtoBusiness = new Mock<IProdutoBusiness>();
            produtoBusiness.Setup(pb => pb.Deletar(idProduto)).Throws(new Exception(Constantes.MensagensErro.ERRO_500));
            var controller = new ProdutosController(produtoBusiness.Object);

            //Act
            var resultado = controller.Deletar(idProduto) as ObjectResult;

            //Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, resultado.StatusCode);
        }

        #endregion
    }
}