using AutoMapper;
using Business.Interface;
using Data.Repositories.NaoRelacional.Interface;
using Data.Repositories.Relacional.Interface;
using Data.Repositories.Relacional.Repository;
using Data.Repositories.XmlTexto.Interfaces;
using Domain.Constantes;
using Domain.DTO;
using Domain.Entidades;
using Domain.Enum;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Xml;

namespace Business.Business
{
    public class ProdutoBusiness : IProdutoBusiness
    {
        private readonly IProdutoRelacionalRepository _produtoRelacionalRepository;
        private readonly IProdutoNaoRelacionalRepository _produtoNaoRelacionalRepository;
        private readonly IProdutoXmlRepository _produtoXmlRepository;
        private readonly IMapper _mapper;

        public ProdutoBusiness(
            IProdutoRelacionalRepository produtoRelacionalRepository,
            IProdutoNaoRelacionalRepository produtoNaoRelacionalRepository,
            IProdutoXmlRepository produtoXmlRepository,
            IMapper mapper)
        {
            _produtoRelacionalRepository = produtoRelacionalRepository;
            _produtoNaoRelacionalRepository = produtoNaoRelacionalRepository;
            _produtoXmlRepository= produtoXmlRepository;
            _mapper = mapper;
        }

        public ProdutoDetalhesDTO Recuperar(Guid? id)
        {
            if(id == null)
                throw new ArgumentNullException($"{Constantes.MensagensErro.ARGUMENTOS_VAZIOS}ID");

            var produto = _produtoRelacionalRepository.Recuperar((Guid)id);
            if (produto != null)
            {
                PersistirDados((int)ERepositorios.RELACIONAL, produto);
                return _mapper.Map<ProdutoDetalhesDTO>(produto);
            }

            produto = VerificarDemaisBases((Guid)id);
            if(produto != null)
                return _mapper.Map<ProdutoDetalhesDTO>(produto);

            throw new KeyNotFoundException(Constantes.MensagensErro.PRODUTO_NAO_ENCONTRADO);
        }

        public IList<ProdutoDetalhesDTO> Listar(int? maximo)
        {
            IList<ProdutoDetalhesDTO> lista = _mapper.Map<IList<ProdutoDetalhesDTO>>(_produtoRelacionalRepository.Listar(maximo));
            if(lista?.Count > 0)
                return lista;

            throw new KeyNotFoundException(Constantes.MensagensErro.PRODUTO_NAO_ENCONTRADO);
        }

        public Guid Criar(ProdutoDTO produtoDTO)
        {
            var entidade = _mapper.Map<Produto>(produtoDTO);
            try
            {
                entidade.IdCompartilhado = Guid.NewGuid();

                _produtoRelacionalRepository.Criar(entidade);
                _produtoNaoRelacionalRepository.Criar(entidade);
                _produtoXmlRepository.Criar(entidade);

                return entidade.IdCompartilhado;
            }
            catch (MongoWriteException)
            {
                SincronizarBases();
                throw;
            }
            catch (XmlException)
            {
                SincronizarBases();
                throw;
            }
            catch (Exception) 
            {
                throw;
            }
        }

        public void Atualizar(Guid? id, ProdutoDTO produtoDTO)
        {
            if(id == null)
                throw new ArgumentNullException($"{Constantes.MensagensErro.ARGUMENTOS_VAZIOS}ID");

            if(produtoDTO == null)
                throw new ArgumentNullException($"{Constantes.MensagensErro.ARGUMENTOS_VAZIOS}Produto");

            _ = Recuperar(id) ?? throw new KeyNotFoundException(Constantes.MensagensErro.PRODUTO_NAO_ENCONTRADO);
            produtoDTO.IdCompartilhado = (Guid)id;

            var entidade = _mapper.Map<Produto>(produtoDTO);
            try
            {
                _produtoRelacionalRepository.Atualizar(entidade);
                _produtoNaoRelacionalRepository.Atualizar(entidade);
                _produtoXmlRepository.Atualizar(entidade);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Deletar(Guid? id)
        {
            if(id == null)
                throw new ArgumentNullException($"{Constantes.MensagensErro.ARGUMENTOS_VAZIOS}ID");

            try
            {
                _produtoRelacionalRepository.Deletar((Guid)id);
                _produtoNaoRelacionalRepository.Deletar((Guid)id);
                _produtoXmlRepository.Deletar((Guid)id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SincronizarBases()
        {
            try
            {
                List<Guid> produtosRelacional = _produtoRelacionalRepository.ListarDadosCompartilhados();
                List<Guid> produtosNaoRelacional = _produtoNaoRelacionalRepository.ListarDadosCompartilhados();
                List<Guid> produtostexto = _produtoXmlRepository.ListarDadosCompartilhados();

                SincronizarRelacional(produtosRelacional, produtosNaoRelacional, produtostexto);
                SincronizarNaoRelacional(produtosRelacional, produtosNaoRelacional, produtostexto);
                SincronizarTexto(produtosRelacional, produtosNaoRelacional, produtostexto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SincronizarRelacional(List<Guid> relacional, List<Guid> naoRelacional, List<Guid> texto)
        {
            IEnumerable<Guid> difRelacionalNaoRelacional = naoRelacional.Except(relacional);
            IEnumerable<Guid> difRelacionalTexto = texto.Except(relacional);

            if (difRelacionalNaoRelacional?.Count() > 0)
            {
                foreach (Guid idDadoNaoRelacional in difRelacionalNaoRelacional)
                {
                    var produto = _produtoNaoRelacionalRepository.Recuperar(idDadoNaoRelacional);
                    _produtoRelacionalRepository.Criar(produto);
                }
            }

            if (difRelacionalTexto?.Count() > 0)
            {
                foreach (Guid idDadoTexto in difRelacionalTexto)
                {
                    var produto = _produtoXmlRepository.Recuperar(idDadoTexto);
                    _produtoRelacionalRepository.Criar(produto);
                }
            }
        }

        private void SincronizarNaoRelacional(List<Guid> relacional, List<Guid> naoRelacional, List<Guid> texto)
        {
            IEnumerable<Guid> difNaoRelacionalRelacional = relacional.Except(naoRelacional);
            IEnumerable<Guid> difNaoRelacionalTexto = texto.Except(naoRelacional);

            if (difNaoRelacionalRelacional?.Count() > 0)
            {
                foreach (Guid idDadoRelacional in difNaoRelacionalRelacional)
                {
                    var produto = _produtoRelacionalRepository.Recuperar(idDadoRelacional);
                    _produtoNaoRelacionalRepository.Criar(produto);
                }
            }

            if (difNaoRelacionalTexto?.Count() > 0)
            {
                foreach (Guid idDadoTexto in difNaoRelacionalTexto)
                {
                    var produto = _produtoXmlRepository.Recuperar(idDadoTexto);
                    _produtoNaoRelacionalRepository.Criar(produto);
                }
            }
        }

        private void SincronizarTexto(List<Guid> relacional, List<Guid> naoRelacional, List<Guid> texto)
        {
            IEnumerable<Guid> difTextoRelacional = relacional.Except(texto);
            IEnumerable<Guid> difTextoNaoRelacional = naoRelacional.Except(texto);

            if (difTextoRelacional?.Count() > 0)
            {
                foreach (Guid idDadoRelacional in difTextoRelacional)
                {
                    var produto = _produtoRelacionalRepository.Recuperar(idDadoRelacional);
                    _produtoXmlRepository.Criar(produto);
                }
            }

            if (difTextoNaoRelacional?.Count() > 0)
            {
                foreach (Guid idDadoNaoRelacional in difTextoNaoRelacional)
                {
                    var produto = _produtoNaoRelacionalRepository.Recuperar(idDadoNaoRelacional);
                    _produtoXmlRepository.Criar(produto);
                }
            }
        }

        private Produto VerificarDemaisBases(Guid id)
        {
            try
            {
                var produtoMongodb = _produtoNaoRelacionalRepository.Recuperar(id);
                if (produtoMongodb != null)
                {
                    PersistirDados((int)ERepositorios.NAO_RELACIONAL, produtoMongodb);
                    return produtoMongodb;
                }

                var produtoTexto = _produtoXmlRepository.Recuperar(id);
                if (produtoTexto != null)
                {
                    PersistirDados((int)ERepositorios.TEXTO, produtoTexto);
                    return produtoTexto;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        private void PersistirDados(int repositorio, Produto produto)
        {
            switch (repositorio)
            {
                case (int)ERepositorios.RELACIONAL:
                    if (_produtoNaoRelacionalRepository.Recuperar(produto.IdCompartilhado) == null)
                        _produtoNaoRelacionalRepository.Criar(produto);
                    if (_produtoXmlRepository.Recuperar(produto.IdCompartilhado) == null)
                        _produtoXmlRepository.Criar(produto);
                    break;
                case (int)ERepositorios.NAO_RELACIONAL:
                    _produtoRelacionalRepository.Criar(produto);
                    if (_produtoXmlRepository.Recuperar(produto.IdCompartilhado) == null)
                        _produtoXmlRepository.Criar(produto);
                    break;
                case (int)ERepositorios.TEXTO:
                    _produtoRelacionalRepository.Criar(produto);
                    _produtoNaoRelacionalRepository.Criar(produto);
                    break;
            }
        }
    }
}
