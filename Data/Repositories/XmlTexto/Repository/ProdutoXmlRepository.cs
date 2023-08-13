﻿using Data.Repositories.XmlTexto.Context;
using Data.Repositories.XmlTexto.Interfaces;
using Domain.Constantes;
using Domain.Entidades;
using SharpCompress.Common;
using SharpCompress.Compressors.Xz;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

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
                EscreverNovoArquivoXml(produto);
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

            produto.Id = IdIncremento();
            EscreverArquivoExistenteXml(produto);
        }

        public IList<Produto> Listar(int? maximo)
        {
            if (new FileInfo(_context.DiretorioXml()).Length == 0)
                return null;

            if (maximo == null)
            {
                using StreamReader stream = new(_context.DiretorioXml());
                try
                {
                    return (IList<Produto>)_context.Serializador().Deserialize(stream);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    stream.Dispose();
                }
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
                    if (leitor.NodeType != XmlNodeType.Element || leitor.Name != "Produto")
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
                leitor.Dispose();
            }
        }

        public void Atualizar(Produto produto)
        {
            if (new FileInfo(_context.DiretorioXml()).Length == 0)
                return;

            string temp = Path.GetTempFileName();
            using XmlReader leitor = XmlReader.Create(_context.DiretorioXml());
            using XmlWriter escreverXml = XmlWriter.Create(temp, _context.ConfigsEscrever());
            try
            {
                escreverXml.WriteStartElement("Produtos");
                new XmlSerializer(typeof(Produto)).Serialize(escreverXml, produto, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
                while (leitor.Read())
                {
                    if (leitor.NodeType != XmlNodeType.Element || leitor.Name != "IdCompartilhado")
                        continue;

                    leitor.Read();
                    if (leitor.NodeType == XmlNodeType.Text)
                    {
                        if (string.IsNullOrWhiteSpace(leitor.Value))
                            continue;
                        int xmlId = int.Parse(leitor.Value);
                        if(xmlId == produto.IdCompartilhado)
                            leitor.Skip();
                    }
                    escreverXml.WriteNode(leitor, true);
                }
                escreverXml.WriteEndElement();
            }
            catch
            {
                throw;
            }
            finally
            {
                escreverXml.Dispose();
                leitor.Dispose();
            }

            FileInfo file = new(_context.DiretorioXml());
            if (XmlBloqueado(file))
                throw new IOException(Constantes.MensagensErro.XML_BLOQUEADO);

            File.Delete(_context.DiretorioXml());
            File.Move(temp, _context.DiretorioXml());
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
                leitor.Dispose();
            }
            return idsCompartilhados;
        }

        private void EscreverNovoArquivoXml(Produto produto)
        {
            using XmlWriter escreverXml = XmlWriter.Create(_context.DiretorioXml(), _context.ConfigsEscrever());
            try
            {
                escreverXml.WriteStartElement("Produtos");
                new XmlSerializer(typeof(Produto)).Serialize(escreverXml, produto, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
                escreverXml.WriteEndElement();
            }
            catch
            {
                throw;
            }
            finally
            {
                escreverXml.Dispose();
            }
        }

        private void EscreverArquivoExistenteXml(Produto produto)
        {
            string temp = Path.GetTempFileName();
            using XmlReader leitor = XmlReader.Create(_context.DiretorioXml());
            using XmlWriter escreverXml = XmlWriter.Create(temp, _context.ConfigsEscrever());
            try
            {
                escreverXml.WriteStartElement("Produtos");
                new XmlSerializer(typeof(Produto)).Serialize(escreverXml, produto, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
                while (leitor.Read())
                {
                    if (leitor.NodeType == XmlNodeType.Element && leitor.Name == "Produto")
                        escreverXml.WriteNode(leitor, true);
                }
                escreverXml.WriteEndElement();
            }
            catch
            {
                throw;
            }
            finally
            {
                escreverXml.Dispose();
                leitor.Dispose();
            }

            FileInfo file = new(_context.DiretorioXml());
            if (XmlBloqueado(file))
                throw new IOException(Constantes.MensagensErro.XML_BLOQUEADO);

            File.Delete(_context.DiretorioXml());
            File.Move(temp, _context.DiretorioXml());
        }

        private bool XmlBloqueado(FileInfo file)
        {
            bool trancado = true;
            int tentativas = 0;
            int maximoTentativas = 3;

            while (trancado && tentativas < maximoTentativas)
            {
                using FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                try
                {
                    stream.Dispose();
                    trancado = false;
                    return trancado;
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                    tentativas++;
                }
                finally
                {
                    stream.Dispose();
                }
            }
            return trancado;
        }

        private int IdIncremento()
        {
            using XmlTextReader leitor = new(_context.DiretorioXml());
            try
            {
                List<int> idsProdutosXml = new();
                while (leitor.Read())
                {
                    if (leitor.NodeType != XmlNodeType.Element || leitor.Name != "Id")
                        continue;

                    leitor.Read();
                    if (leitor.NodeType == XmlNodeType.Text)
                    {
                        string xmlId = leitor.Value;
                        if (!string.IsNullOrWhiteSpace(xmlId))
                            idsProdutosXml.Add(int.Parse(xmlId));
                    }
                }
                return idsProdutosXml.Max() + 1;
            }
            catch
            {
                throw;
            }
            finally
            {
                leitor.Dispose();
            }
        }
    }
}
