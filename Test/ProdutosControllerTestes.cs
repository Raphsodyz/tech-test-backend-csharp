using AutoMapper;
using Business.Interface;
using Business.Mapper;
using Domain.DTO;
using Domain.Entidades;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Controllers;
using System.ComponentModel.DataAnnotations;

namespace Test
{
    [TestClass]
    public class ProdutosControllerTestes
    {
        #region Valida��o objeto

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
            Assert.AreEqual(erros[0].ErrorMessage, "O valor enviado no campo Pre�o � inv�lido.");
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
            Assert.AreEqual(erros[0].ErrorMessage, "O campo Nome � de preenchimento obrigat�rio.");
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
            Assert.AreEqual(erros[0].ErrorMessage, "O campo Pre�o � de preenchimento obrigat�rio.");
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
            Assert.AreEqual(erros[0].ErrorMessage, "O campo Quantidade � de preenchimento obrigat�rio.");
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
            Assert.AreEqual(erros[0].ErrorMessage, "O campo Data de Cria��o � de preenchimento obrigat�rio.");
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

        #region Controller a��es

        [TestMethod]
        public void ProdutosController_RecuperarProduto_ProdutoEncontrado()
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

        #endregion
    }
}