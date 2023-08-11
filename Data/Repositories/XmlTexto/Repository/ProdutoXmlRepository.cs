using Data.Repositories.XmlTexto.Context;
using Data.Repositories.XmlTexto.Interfaces;
using Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Data.Repositories.XmlTexto.Repository
{
    public class ProdutoXmlRepository : IProdutoXmlRepository
    {
        private readonly MobilusXmlContext _context;
        public ProdutoXmlRepository(MobilusXmlContext context)
        {
            _context = context;
        }

        public void Criar(Produto produto)
        {
            if(produto.Id == 0)
            {
                using XmlTextReader leitor = new(_context.DiretorioXml());
                try
                {
                    List<int> produtosIdsXml = new();
                    while (leitor.Read())
                    {
                        if (leitor.NodeType == XmlNodeType.Element && leitor.Name == "Produto")
                        {
                            string xmlId = leitor.GetAttribute("Id");
                            if (!string.IsNullOrWhiteSpace(xmlId))
                                produtosIdsXml.Add(int.Parse(xmlId));
                        }
                    }
                    produto.Id = produtosIdsXml.Max() + 1;
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    leitor.Dispose();
                }
            }

            StringWriter escreverTexto = new();
            _context.Serializador().Serialize(escreverTexto, produto);
            string produtoXml = escreverTexto.ToString();

            using XmlWriter escreverXml = XmlWriter.Create(_context.DiretorioXml(), _context.ConfigsEscrever());
            escreverXml.WriteStartElement("Produto");
            escreverXml.WriteRaw(produtoXml);
            escreverXml.WriteEndElement();
        }

        public IList<Produto> Listar(int? maximo)
        {
            if (maximo == null)
            {
                using StreamReader stream = new(_context.DiretorioXml());
                return (IList<Produto>)_context.Serializador().Deserialize(stream);
            }

            int contagem = 0;
            IList<Produto> produtos = new List<Produto>();
            using XmlTextReader leitor = new(_context.DiretorioXml());
            try
            {
                while (leitor.Read())
                {
                    if (leitor.NodeType == XmlNodeType.Element && leitor.Name == "Produto")
                    {
                        if (contagem == maximo)
                            return produtos;

                        produtos.Add((Produto)_context.Serializador().Deserialize(leitor.ReadSubtree()));
                        contagem++;
                    }
                }
                return produtos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                leitor.Dispose();
            }
        }

        public Produto Recuperar(int id)
        {
            using XmlTextReader leitor = new(_context.DiretorioXml());
            try
            {
                while (leitor.Read())
                {
                    if (leitor.NodeType == XmlNodeType.Element && leitor.Name == "Produto")
                    {
                        Produto temporario = (Produto)_context.Serializador().Deserialize(leitor.ReadSubtree());
                        if (temporario?.IdCompartilhado == id)
                            return temporario;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                leitor.Dispose();
            }
        }

        public void Atualizar(Produto produto)
        {
            throw new NotImplementedException();
        }        

        public void Deletar(int id)
        {
            throw new NotImplementedException();
        }
    }
}
