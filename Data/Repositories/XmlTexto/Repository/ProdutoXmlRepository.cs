using Data.Repositories.XmlTexto.Context;
using Data.Repositories.XmlTexto.Interfaces;
using Domain.Entidades;
using SharpCompress.Writers;
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
            if (new FileInfo(_context.DiretorioXml()).Length == 0)
            {
                produto.Id = 1;
                NovoProduto(produto);
                return;
            }

            if (produto.IdCompartilhado != 0)
            {
                var produtoExist = Recuperar((int)produto.IdCompartilhado);
                if(produtoExist != null)
                {
                    Atualizar(produto);
                    return;
                }
            }

            if (produto.Id == 0)
                produto.Id = IdIncremento();

            NovoProduto(produto);
        }

        public IList<Produto> Listar(int? maximo)
        {
            if (new FileInfo(_context.DiretorioXml()).Length == 0)
                return null;

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
                    if (leitor.NodeType != XmlNodeType.Element && leitor.Name != "Produto")
                        continue;

                    if (contagem == maximo)
                        return produtos;

                    produtos.Add((Produto)_context.Serializador().Deserialize(leitor.ReadSubtree()));
                    contagem++;
                }
                return produtos;
            }
            catch
            {
                throw;
            }
            finally
            {
                leitor.Close();
                leitor.Dispose();
            }
        }

        public Produto Recuperar(int id)
        {
            if (new FileInfo(_context.DiretorioXml()).Length == 0)
                return null;

            using XmlTextReader leitor = new(_context.DiretorioXml());
            try
            {
                while (leitor.Read())
                {
                    if (leitor.NodeType != XmlNodeType.Element && leitor.Name != "Produto")
                        continue;

                    Produto produto = (Produto)_context.Serializador().Deserialize(leitor.ReadSubtree());
                    if (produto?.IdCompartilhado == id)
                        return produto;
                }
                return null;
            }
            catch 
            { 
                throw; 
            }
            finally
            {
                leitor.Close();
                leitor.Dispose();
            }
        }

        public void Atualizar(Produto produto)
        {
            if (new FileInfo(_context.DiretorioXml()).Length == 0)
                return;

            using XmlTextReader leitor = new(_context.DiretorioXml());
            using XmlWriter escreverXml = XmlWriter.Create(_context.DiretorioXml(), _context.ConfigsEscrever());
            try
            {
                while (leitor.Read())
                {
                    if (leitor.NodeType != XmlNodeType.Element && leitor.Name != "Produto")
                        continue;

                    int idCompartilhado = int.Parse(leitor.GetAttribute("IdCompartilhado"));
                    if (idCompartilhado != produto.IdCompartilhado)
                        continue;

                    escreverXml.WriteStartElement("Produto");

                    escreverXml.WriteElementString("Nome", produto.Nome);
                    escreverXml.WriteElementString("Preco", produto.Preco.ToString());
                    escreverXml.WriteElementString("Quantidade", produto.Quantidade.ToString());
                    escreverXml.WriteElementString("DataCriacao", produto.DataCriacao.ToString());

                    escreverXml.WriteEndElement();
                    break;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                escreverXml.Close();
                escreverXml.Dispose();
                leitor.Close();
                leitor.Dispose();
            }
        }        

        public void Deletar(int id)
        {
            if (new FileInfo(_context.DiretorioXml()).Length == 0)
                return;

            XmlDocument documento = new XmlDocument();
            documento.Load(_context.DiretorioXml());

            XmlNodeList nodos = documento.SelectNodes("Produtos");
            foreach (XmlNode nodo in nodos)
            {
                int idCompartilhado = int.Parse(nodo.SelectSingleNode("Id").InnerText);
                if (idCompartilhado == id)
                {
                    nodo.ParentNode.RemoveChild(nodo);
                    break;
                }
            }
        }

        public List<int> ListarDadosCompartilhados()
        {
            if (new FileInfo(_context.DiretorioXml()).Length == 0)
                return null;

            List<int> idsCompartilhados = new();
            using XmlTextReader leitor = new(_context.DiretorioXml());
            try
            {
                while (leitor.Read())
                {
                    Produto temporario = null;
                    if (leitor.NodeType == XmlNodeType.Element)
                    {
                        if (leitor.Name == "Produto")
                            temporario = new Produto();
                        else if (leitor.Name == "IdCompartilhado" && temporario != null)
                        {
                            if (leitor.Read())
                                temporario.IdCompartilhado = int.Parse(leitor.Value);
                        }
                    }
                    else if (leitor.NodeType == XmlNodeType.EndElement && leitor.Name == "Produto" && temporario != null)
                    {
                        idsCompartilhados.Add(temporario.IdCompartilhado);
                        temporario = null;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                leitor.Close();
                leitor.Dispose();
            }
            return idsCompartilhados;
        }

        private void NovoProduto(Produto produto)
        {
            StringWriter escreverTexto = new();
            _context.Serializador().Serialize(escreverTexto, produto);
            string produtoXml = escreverTexto.ToString();

            using XmlWriter escreverXml = XmlWriter.Create(_context.DiretorioXml(), _context.ConfigsEscrever());
            try
            {
                escreverXml.WriteStartElement("Produto");
                escreverXml.WriteRaw(produtoXml);
                escreverXml.WriteEndElement();
            }
            catch
            {
                throw;
            }
            finally
            {
                escreverXml.Close();
                escreverXml.Dispose();
            }
        }

        private int IdIncremento()
        {
            using XmlTextReader leitor = new(_context.DiretorioXml());
            try
            {
                List<int> idsProdutosXml = new();
                while (leitor.Read())
                {
                    if (leitor.NodeType != XmlNodeType.Element && leitor.Name != "Produto")
                        continue;

                    string xmlId = leitor.GetAttribute("Id");
                    if (!string.IsNullOrWhiteSpace(xmlId))
                        idsProdutosXml.Add(int.Parse(xmlId));
                }
                return idsProdutosXml.Max() + 1;
            }
            catch
            {
                throw;
            }
            finally
            {
                leitor.Close();
                leitor.Dispose();
            }
        }
    }
}
