using AutoMapper;
using Business.Business;
using Business.Interface;
using Data.Repositories.NaoRelacional.Interface;
using Data.Repositories.Relacional.Interface;
using Data.Repositories.XmlTexto.Interfaces;
using Domain.Constantes;
using Domain.DTO;
using Domain.Entidades;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Test
{
    [TestClass]
    public class ProdutoBusinessTestes
    {
        [TestMethod]
        public void ProdutoBusiness_Recuperar_ProdutoEncontradoTodosBancos()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns(produto);
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns(produto);
            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns(produto);
            mapper.Setup(m => m.Map<ProdutoDetalhesDTO>(produto)).Returns(produtoDetalhesDTO);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = produtoBusiness.Recuperar(produto.IdCompartilhado);

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, produtoDetalhesDTO);
        }

        [TestMethod]
        public void ProdutoBusiness_Recuperar_ProdutoFaltandoRelacional()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoRelacionalRepository.Setup(prr => prr.Criar(produto));

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns(produto);
            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns(produto);
            mapper.Setup(m => m.Map<ProdutoDetalhesDTO>(produto)).Returns(produtoDetalhesDTO);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = produtoBusiness.Recuperar(produto.IdCompartilhado);

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, produtoDetalhesDTO);
        }

        [TestMethod]
        public void ProdutoBusiness_Recuperar_ProdutoFaltandoRelacionalXmlTexto()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoRelacionalRepository.Setup(prr => prr.Criar(produto));

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoXmlRepository.Setup(pxr => pxr.Criar(produto));

            mapper.Setup(m => m.Map<ProdutoDetalhesDTO>(produto)).Returns(produtoDetalhesDTO);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = produtoBusiness.Recuperar(produto.IdCompartilhado);

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, produtoDetalhesDTO);
        }

        [TestMethod]
        public void ProdutoBusiness_Recuperar_ProdutoFaltandoRelacionalNaoRelacional()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoRelacionalRepository.Setup(prr => prr.Criar(produto));

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Criar(produto));

            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            mapper.Setup(m => m.Map<ProdutoDetalhesDTO>(produto)).Returns(produtoDetalhesDTO);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = produtoBusiness.Recuperar(produto.IdCompartilhado);

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, produtoDetalhesDTO);
        }

        [TestMethod]
        public void ProdutoBusiness_Recuperar_ProdutoNaoEncontrado()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);
            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns((Produto)null);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Recuperar(produto.IdCompartilhado);

            //Assert
            Assert.ThrowsException<KeyNotFoundException>(resultado);
        }

        [TestMethod]
        public void ProdutoBusiness_Listar_RetornarListaProdutos()
        {
            //Arrange
            IList<ProdutoDetalhesDTO> produtosDetalhe = new List<ProdutoDetalhesDTO>()
            {
                new()
                {
                    Nome = "Caneta",
                    Preco = 5,
                    DataCriacao = DateTime.Now,
                    Quantidade = 10,
                    ValorTotal = 5 * 10,
                    Codigo = Guid.NewGuid()
                },

                new()
                {
                    Nome = "Lápis",
                    Preco = 3,
                    DataCriacao = DateTime.Now,
                    Quantidade = 8,
                    ValorTotal = 3 * 8,
                    Codigo = Guid.NewGuid()
                },

                new()
                {
                    Nome = "Borracha",
                    Preco = 2,
                    DataCriacao = DateTime.Now,
                    Quantidade = 8,
                    ValorTotal = 2 * 8,
                    Codigo = Guid.NewGuid()
                }
            };
            IList<Produto> produtos = new List<Produto>()
            {
                new()
                {
                    Nome = "Caneta",
                    Preco = 5,
                    DataCriacao = DateTime.Now,
                    Quantidade = 10,
                    IdCompartilhado = Guid.NewGuid()
                },

                new()
                {
                    Nome = "Lápis",
                    Preco = 3,
                    DataCriacao = DateTime.Now,
                    Quantidade = 8,
                    IdCompartilhado = Guid.NewGuid()
                },

                new()
                {
                    Nome = "Borracha",
                    Preco = 2,
                    DataCriacao = DateTime.Now,
                    Quantidade = 8,
                    IdCompartilhado = Guid.NewGuid()
                }
            };

            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.ListarDadosCompartilhados()).Returns(new List<Guid>());
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.ListarDadosCompartilhados()).Returns(new List<Guid>());
            produtoXmlRepository.Setup(pxr => pxr.ListarDadosCompartilhados()).Returns(new List<Guid>());

            produtoRelacionalRepository.Setup(prr => prr.Listar(null)).Returns(produtos);
            mapper.Setup(m => m.Map<IList<ProdutoDetalhesDTO>>(produtos)).Returns(produtosDetalhe);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = produtoBusiness.Listar(null);

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, produtosDetalhe);
            Assert.AreEqual(resultado.Count, 3);
        }

        [TestMethod]
        public void ProdutoBusiness_Listar_ListaComMaximo()
        {
            //Arrange
            IList<ProdutoDetalhesDTO> produtosDetalhe = new List<ProdutoDetalhesDTO>()
            {
                new()
                {
                    Nome = "Caneta",
                    Preco = 5,
                    DataCriacao = DateTime.Now,
                    Quantidade = 10,
                    ValorTotal = 5 * 10,
                    Codigo = Guid.NewGuid()
                },

                new()
                {
                    Nome = "Lápis",
                    Preco = 3,
                    DataCriacao = DateTime.Now,
                    Quantidade = 8,
                    ValorTotal = 3 * 8,
                    Codigo = Guid.NewGuid()
                },

                new()
                {
                    Nome = "Borracha",
                    Preco = 2,
                    DataCriacao = DateTime.Now,
                    Quantidade = 8,
                    ValorTotal = 2 * 8,
                    Codigo = Guid.NewGuid()
                }
            };
            IList<Produto> produtos = new List<Produto>()
            {
                new()
                {
                    Nome = "Caneta",
                    Preco = 5,
                    DataCriacao = DateTime.Now,
                    Quantidade = 10,
                    IdCompartilhado = Guid.NewGuid()
                },

                new()
                {
                    Nome = "Lápis",
                    Preco = 3,
                    DataCriacao = DateTime.Now,
                    Quantidade = 8,
                    IdCompartilhado = Guid.NewGuid()
                },

                new()
                {
                    Nome = "Borracha",
                    Preco = 2,
                    DataCriacao = DateTime.Now,
                    Quantidade = 8,
                    IdCompartilhado = Guid.NewGuid()
                }
            };
            int maximo = 2;
            IList<Produto> produtosMaximos = produtos.Take(maximo).ToList();
            IList<ProdutoDetalhesDTO> produtosDetalheMaximos = produtosDetalhe.Take(maximo).ToList();

            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.ListarDadosCompartilhados()).Returns(new List<Guid>());
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.ListarDadosCompartilhados()).Returns(new List<Guid>());
            produtoXmlRepository.Setup(pxr => pxr.ListarDadosCompartilhados()).Returns(new List<Guid>());

            produtoRelacionalRepository.Setup(prr => prr.Listar(maximo)).Returns(produtosMaximos);
            mapper.Setup(m => m.Map<IList<ProdutoDetalhesDTO>>(produtosMaximos)).Returns(produtosDetalheMaximos);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = produtoBusiness.Listar(maximo);

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, produtosDetalheMaximos);
            Assert.AreEqual(resultado.Count, maximo);
        }

        [TestMethod]
        public void ProdutoBusiness_Listar_RetornarException()
        {
            //Arrange
            IList<ProdutoDetalhesDTO> produtosDetalhe = new List<ProdutoDetalhesDTO>();
            IList<Produto> produtos = new List<Produto>();

            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.ListarDadosCompartilhados()).Returns(new List<Guid>());
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.ListarDadosCompartilhados()).Returns(new List<Guid>());
            produtoXmlRepository.Setup(pxr => pxr.ListarDadosCompartilhados()).Returns(new List<Guid>());

            produtoRelacionalRepository.Setup(prr => prr.Listar(null)).Returns(produtos);
            mapper.Setup(m => m.Map<IList<ProdutoDetalhesDTO>>(produtos)).Returns(produtosDetalhe);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Listar(null);

            //Assert
            Assert.ThrowsException<KeyNotFoundException>(resultado);
        }

        [TestMethod]
        public void ProdutoBusiness_Criar_CriarNovoProduto()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Criar(produto));
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Criar(produto));
            produtoXmlRepository.Setup(pxr => pxr.Criar(produto));

            mapper.Setup(m => m.Map<Produto>(produtoDTO)).Returns(produto);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = produtoBusiness.Criar(produtoDTO);
            produtoDTO.IdCompartilhado = resultado;

            //Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(resultado, produtoDTO.IdCompartilhado);
        }

        [TestMethod]
        public void ProdutoBusiness_Criar_MongoException()
        {
            //Arrange
            var connectionId = new ConnectionId(new ServerId(new ClusterId(1), new DnsEndPoint("localhost", 27017)), 2);
            var innerException = new Exception(string.Empty);
            var ctor = typeof(WriteConcernError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeConcernError = (WriteConcernError)ctor.Invoke(new object[] { 1, "writeConcernError", "writeConcernError", new BsonDocument("details", "writeConcernError"), new List<string>() });
            ctor = typeof(WriteError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeError = (WriteError)ctor.Invoke(new object[] { ServerErrorCategory.Uncategorized, 1, "writeError", new BsonDocument("details", "writeError") });
            var exception = new MongoWriteException(connectionId, writeError, writeConcernError, innerException);
            
            Guid guid = Guid.NewGuid();

            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Criar(produto));
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Criar(produto)).Throws(new MongoWriteException(connectionId, writeError, writeConcernError, innerException));
            produtoXmlRepository.Setup(pxr => pxr.Criar(produto));

            mapper.Setup(m => m.Map<Produto>(produtoDTO)).Returns(produto);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Criar(produtoDTO);

            //Assert
            Assert.ThrowsException<Exception>(() => produtoBusiness.Criar(produtoDTO));
        }

        [TestMethod]
        public void ProdutoBusiness_Criar_XmlException()
        {
            //Arrange
            Guid guid = Guid.NewGuid();

            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Criar(produto));
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Criar(produto));
            produtoXmlRepository.Setup(pxr => pxr.Criar(produto)).Throws(new XmlException(Constantes.MensagensErro.ERRO_XML));

            mapper.Setup(m => m.Map<Produto>(produtoDTO)).Returns(produto);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Criar(produtoDTO);

            //Assert
            Assert.ThrowsException<Exception>(() => produtoBusiness.Criar(produtoDTO));
        }

        [TestMethod]
        public void ProdutoBusiness_Criar_ErroInterno()
        {
            //Arrange
            Guid guid = Guid.NewGuid();

            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Criar(produto)).Throws(new Exception(string.Empty));
            mapper.Setup(m => m.Map<Produto>(produtoDTO)).Returns(produto);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Criar(produtoDTO);

            //Assert
            Assert.ThrowsException<Exception>(() => produtoBusiness.Criar(produtoDTO));
        }

        [TestMethod]
        public void ProdutoBusiness_Atualizar_ProdutoAtualizado()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Atualizar(produto));
            produtoRelacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Atualizar(produto));
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            produtoXmlRepository.Setup(pxr => pxr.Atualizar(produto));
            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            mapper.Setup(m => m.Map<Produto>(produtoDTO)).Returns(produto);
            mapper.Setup(m => m.Map<ProdutoDetalhesDTO>(produto)).Returns(produtoDetalhesDTO);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            produtoBusiness.Atualizar(produto.IdCompartilhado, produtoDTO);

            //Assert
            produtoRelacionalRepository.Verify(prr => prr.Atualizar(produto), Times.Once);
            produtoNaoRelacionalRepository.Verify(pnrr => pnrr.Atualizar(produto), Times.Once);
            produtoXmlRepository.Verify(pxr => pxr.Atualizar(produto), Times.Once);
        }

        [TestMethod]
        public void ProdutoBusiness_Atualizar_CampoIdVazio()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Atualizar(null, produtoDTO);

            //Assert
            Assert.ThrowsException<ArgumentNullException>(resultado);
        }

        [TestMethod]
        public void ProdutoBusiness_Atualizar_SemProdutoRequisicao()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Atualizar(produtoDTO.IdCompartilhado, null);

            //Assert
            Assert.ThrowsException<ArgumentNullException>(resultado);
        }

        [TestMethod]
        public void ProdutoBusiness_Atualizar_ErroMongoDb()
        {
            //Arrange
            var connectionId = new ConnectionId(new ServerId(new ClusterId(1), new DnsEndPoint("localhost", 27017)), 2);
            var innerException = new Exception(string.Empty);
            var ctor = typeof(WriteConcernError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeConcernError = (WriteConcernError)ctor.Invoke(new object[] { 1, "writeConcernError", "writeConcernError", new BsonDocument("details", "writeConcernError"), new List<string>() });
            ctor = typeof(WriteError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeError = (WriteError)ctor.Invoke(new object[] { ServerErrorCategory.Uncategorized, 1, "writeError", new BsonDocument("details", "writeError") });
            var exception = new MongoWriteException(connectionId, writeError, writeConcernError, innerException);

            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Atualizar(produto));
            produtoRelacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Atualizar(produto)).Throws(new MongoWriteException(connectionId, writeError, writeConcernError, innerException));
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            mapper.Setup(m => m.Map<Produto>(produtoDTO)).Returns(produto);
            mapper.Setup(m => m.Map<ProdutoDetalhesDTO>(produto)).Returns(produtoDetalhesDTO);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Atualizar(produto.IdCompartilhado, produtoDTO);

            //Assert
            Assert.ThrowsException<Exception>(resultado);
        }

        [TestMethod]
        public void ProdutoBusiness_Atualizar_ErroXml()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Atualizar(produto));
            produtoRelacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Atualizar(produto));
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            produtoXmlRepository.Setup(pxr => pxr.Atualizar(produto)).Throws(new XmlException(string.Empty));
            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            mapper.Setup(m => m.Map<Produto>(produtoDTO)).Returns(produto);
            mapper.Setup(m => m.Map<ProdutoDetalhesDTO>(produto)).Returns(produtoDetalhesDTO);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Atualizar(produto.IdCompartilhado, produtoDTO);

            //Assert
            Assert.ThrowsException<Exception>(resultado);
        }

        [TestMethod]
        public void ProdutoBusiness_Atualizar_ErroInternoRelacional()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Atualizar(produto)).Throws(new Exception(string.Empty));
            produtoRelacionalRepository.Setup(prr => prr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produto.IdCompartilhado)).Returns(produto);
            produtoXmlRepository.Setup(pxr => pxr.Recuperar(produto.IdCompartilhado)).Returns(produto);

            mapper.Setup(m => m.Map<Produto>(produtoDTO)).Returns(produto);
            mapper.Setup(m => m.Map<ProdutoDetalhesDTO>(produto)).Returns(produtoDetalhesDTO);

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Atualizar(produto.IdCompartilhado, produtoDTO);

            //Assert
            Assert.ThrowsException<Exception>(resultado);
        }

        [TestMethod]
        public void ProdutoBusiness_Deletar_ProdutoDeletado()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Deletar(produto.IdCompartilhado));
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Deletar(produto.IdCompartilhado));
            produtoXmlRepository.Setup(pxr => pxr.Deletar(produto.IdCompartilhado));

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            produtoBusiness.Deletar(produto.IdCompartilhado);

            //Assert
            produtoRelacionalRepository.Verify(prr => prr.Deletar(produto.IdCompartilhado), Times.Once);
            produtoNaoRelacionalRepository.Verify(pnrr => pnrr.Deletar(produto.IdCompartilhado), Times.Once);
            produtoXmlRepository.Verify(pxr => pxr.Deletar(produto.IdCompartilhado), Times.Once);
        }

        [TestMethod]
        public void ProdutoBusiness_Deletar_ArgumentoVazio()
        {
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Deletar(null);

            //Assert
            Assert.ThrowsException<ArgumentNullException>(resultado);
        }

        [TestMethod]
        public void ProdutoBusiness_Deletar_MongoDbErro()
        {
            //Arrange
            var connectionId = new ConnectionId(new ServerId(new ClusterId(1), new DnsEndPoint("localhost", 27017)), 2);
            var innerException = new Exception(string.Empty);
            var ctor = typeof(WriteConcernError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeConcernError = (WriteConcernError)ctor.Invoke(new object[] { 1, "writeConcernError", "writeConcernError", new BsonDocument("details", "writeConcernError"), new List<string>() });
            ctor = typeof(WriteError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeError = (WriteError)ctor.Invoke(new object[] { ServerErrorCategory.Uncategorized, 1, "writeError", new BsonDocument("details", "writeError") });
            var exception = new MongoWriteException(connectionId, writeError, writeConcernError, innerException);

            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Deletar(produto.IdCompartilhado));
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Deletar(produto.IdCompartilhado)).Throws(new MongoWriteException(connectionId, writeError, writeConcernError, innerException));
            produtoXmlRepository.Setup(pxr => pxr.Deletar(produto.IdCompartilhado));

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Deletar(produto.IdCompartilhado);

            //Assert
            Assert.ThrowsException<Exception>(resultado);
        }

        [TestMethod]
        public void ProdutoBusiness_Deletar_XmlErro()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Deletar(produto.IdCompartilhado));
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Deletar(produto.IdCompartilhado));
            produtoXmlRepository.Setup(pxr => pxr.Deletar(produto.IdCompartilhado)).Throws(new XmlException(string.Empty)); ;

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Deletar(produto.IdCompartilhado);

            //Assert
            Assert.ThrowsException<Exception>(resultado);
        }

        [TestMethod]
        public void ProdutoBusiness_Deletar_ErroInternoRelacional()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.Deletar(produto.IdCompartilhado)).Throws(new Exception(string.Empty));
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Deletar(produto.IdCompartilhado));
            produtoXmlRepository.Setup(pxr => pxr.Deletar(produto.IdCompartilhado)); ;

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.Deletar(produto.IdCompartilhado);

            //Assert
            Assert.ThrowsException<Exception>(resultado);
        }

        [TestMethod]
        public void ProdutoBusiness_SincronizarBases_BasesSincronizadas()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            List<Guid> produtos = new() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            produtoRelacionalRepository.Setup(prr => prr.ListarDadosCompartilhados()).Returns(produtos);
            produtoRelacionalRepository.Setup(prr => prr.Recuperar(produtos[0])).Returns(produto);
            produtoRelacionalRepository.Setup(prr => prr.Criar(produto));

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.ListarDadosCompartilhados()).Returns(produtos);
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produtos[0])).Returns(produto);
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Criar(produto));

            produtoXmlRepository.Setup(pxr => pxr.ListarDadosCompartilhados()).Returns(produtos);
            produtoXmlRepository.Setup(pnrr => pnrr.Recuperar(produtos[0])).Returns(produto);
            produtoXmlRepository.Setup(pnrr => pnrr.Criar(produto));

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            produtoBusiness.SincronizarBases();

            //Assert
            produtoRelacionalRepository.Verify(prr => prr.ListarDadosCompartilhados(), Times.Once);
            produtoNaoRelacionalRepository.Verify(pnrr => pnrr.ListarDadosCompartilhados(), Times.Once);
            produtoXmlRepository.Verify(pxr => pxr.ListarDadosCompartilhados(), Times.Once);
        }

        [TestMethod]
        public void ProdutoBusiness_SincronizarBases_ComDadoFaltandoEmBase()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            List<Guid> produtos = new() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            produtoRelacionalRepository.Setup(prr => prr.ListarDadosCompartilhados()).Returns(produtos);
            produtoRelacionalRepository.Setup(prr => prr.Recuperar(produtos[0])).Returns(produto);
            produtoRelacionalRepository.Setup(prr => prr.Criar(produto));

            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.ListarDadosCompartilhados()).Returns(produtos);
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Recuperar(produtos[0])).Returns(produto);
            produtoNaoRelacionalRepository.Setup(pnrr => pnrr.Criar(produto));

            produtoXmlRepository.Setup(pxr => pxr.ListarDadosCompartilhados()).Returns(produtos.Take(2).ToList());
            produtoXmlRepository.Setup(pnrr => pnrr.Recuperar(produtos[0])).Returns(produto);
            produtoXmlRepository.Setup(pnrr => pnrr.Criar(produto));

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            produtoBusiness.SincronizarBases();

            //Assert
            produtoRelacionalRepository.Verify(prr => prr.ListarDadosCompartilhados(), Times.Once);
            produtoNaoRelacionalRepository.Verify(pnrr => pnrr.ListarDadosCompartilhados(), Times.Once);
            produtoXmlRepository.Verify(pxr => pxr.ListarDadosCompartilhados(), Times.Once);
        }

        [TestMethod]
        public void ProdutoBusiness_SincronizarBases_Exception()
        {
            //Arrange
            var produtoRelacionalRepository = new Mock<IProdutoRelacionalRepository>();
            var produtoNaoRelacionalRepository = new Mock<IProdutoNaoRelacionalRepository>();
            var produtoXmlRepository = new Mock<IProdutoXmlRepository>();
            var mapper = new Mock<IMapper>();

            produtoRelacionalRepository.Setup(prr => prr.ListarDadosCompartilhados()).Throws(new Exception(string.Empty));

            var produtoBusiness = new ProdutoBusiness(produtoRelacionalRepository.Object, produtoNaoRelacionalRepository.Object, produtoXmlRepository.Object, mapper.Object);

            //Act
            var resultado = () => produtoBusiness.SincronizarBases();

            //Assert
            Assert.ThrowsException<Exception>(resultado);
        }

        #region Objetos Auxiliares

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
            Quantidade = 10
        };

        private readonly ProdutoDetalhesDTO produtoDetalhesDTO = new()
        {
            Nome = "Caneta",
            Preco = 5,
            DataCriacao = DateTime.Now,
            Quantidade = 10,
            Codigo = Guid.NewGuid(),
            ValorTotal = 5 * 10
        };

        #endregion
    }
}
