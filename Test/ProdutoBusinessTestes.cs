using AutoMapper;
using Business.Business;
using Business.Interface;
using Data.Repositories.NaoRelacional.Interface;
using Data.Repositories.Relacional.Interface;
using Data.Repositories.XmlTexto.Interfaces;
using Domain.DTO;
using Domain.Entidades;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class ProdutoBusinessTestes
    {

        private readonly Produto produto = new()
        {
            Id = Guid.NewGuid(),
            Nome = "Caneta",
            Preco = 5,
            DataCriacao = DateTime.Now,
            Quantidade = 10,
            IdCompartilhado = Guid.NewGuid(),
        };

        private readonly ProdutoDTO produtoDTO = new()
        {
            Nome = "Caneta",
            Preco = 5,
            DataCriacao = DateTime.Now,
            Quantidade = 10,
            ValorTotal = 5 * 10,
            IdCompartilhado = Guid.NewGuid()
        };

        [TestMethod]
        public void ProdutoBusiness_Recuperar_ProdutoEncontradoTodosBancos()
        {
            //Arrange
            var produtoReplacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoReplacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns(produto);
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns(produto);
            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns(produto);
            mapper.Setup(m => m.Map<ProdutoDTO>(produto)).Returns(produtoDTO);

            var produtoBusiness = new ProdutoBusiness(produtoReplacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = produtoBusiness.Recuperar(produto.IdCompartilhado);

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, produtoDTO);
        }

        [TestMethod]
        public void ProdutoBusiness_Recuperar_ProdutoFaltandoRelacional()
        {
            //Arrange
            var produtoReplacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoReplacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoReplacionalRepository.Setup(prr => prr.Criar(produto));

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns(produto);
            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns(produto);
            mapper.Setup(m => m.Map<ProdutoDTO>(produto)).Returns(produtoDTO);

            var produtoBusiness = new ProdutoBusiness(produtoReplacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = produtoBusiness.Recuperar(produto.IdCompartilhado);

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, produtoDTO);
        }

        [TestMethod]
        public void ProdutoBusiness_Recuperar_ProdutoFaltandoRelacionalXmlTexto()
        {
            //Arrange
            var produtoReplacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoReplacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoReplacionalRepository.Setup(prr => prr.Criar(produto));

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoXmlRepository.Setup(pxr => pxr.Criar(produto));

            mapper.Setup(m => m.Map<ProdutoDTO>(produto)).Returns(produtoDTO);

            var produtoBusiness = new ProdutoBusiness(produtoReplacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = produtoBusiness.Recuperar(produto.IdCompartilhado);

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, produtoDTO);
        }

        [TestMethod]
        public void ProdutoBusiness_Recuperar_ProdutoFaltandoRelacionalNaoRelacional()
        {
            //Arrange
            var produtoReplacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoReplacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoReplacionalRepository.Setup(prr => prr.Criar(produto));

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Criar(produto));

            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            mapper.Setup(m => m.Map<ProdutoDTO>(produto)).Returns(produtoDTO);

            var produtoBusiness = new ProdutoBusiness(produtoReplacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = produtoBusiness.Recuperar(produto.IdCompartilhado);

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, produtoDTO);
        }

        [TestMethod]
        public void ProdutoBusiness_Recuperar_ProdutoFaltandoSomente()
        {
            //Arrange
            var produtoReplacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoReplacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoReplacionalRepository.Setup(prr => prr.Criar(produto));

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            mapper.Setup(m => m.Map<ProdutoDTO>(produto)).Returns(produtoDTO);

            var produtoBusiness = new ProdutoBusiness(produtoReplacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = produtoBusiness.Recuperar(produto.IdCompartilhado);

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, produtoDTO);
        }
    }
}
