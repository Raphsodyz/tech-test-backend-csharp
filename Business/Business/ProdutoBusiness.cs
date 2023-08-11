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

        public ProdutoDTO Recuperar(int? id)
        {
            if(id == null)
                throw new ArgumentNullException($"{Constantes.MensagensErro.ARGUMENTOS_VAZIOS}ID");

            var produto = _produtoRelacionalRepository.Recuperar((int)id);
            if (produto != null)
                return _mapper.Map<ProdutoDTO>(produto);

            produto = VerificarDemaisBases((int)id);
            if(produto != null)
                return _mapper.Map<ProdutoDTO>(produto);

            throw new KeyNotFoundException(Constantes.MensagensErro.PRODUTO_NAO_ENCONTRADO);
        }

        public IList<ProdutoDTO> Listar(int? maximo)
        {
            return _mapper.Map<IList<ProdutoDTO>>(_produtoRelacionalRepository.Listar(maximo));
        }

        public void Criar(ProdutoDTO entidade)
        {
            
        }

        public void Atualizar(ProdutoDTO produto)
        {
            throw new NotImplementedException();
        }

        public void Deletar(int? id)
        {
            throw new NotImplementedException();
        }

        public void SincronizarBases()
        {
            try
            {
                List<int> produtosRelacional = _produtoRelacionalRepository.ListarDadosCompartilhados();
                List<int> produtosNaoRelacional = _produtoNaoRelacionalRepository.ListarDadosCompartilhados();
                List<int> produtostexto = _produtoXmlRepository.ListarDadosCompartilhados();

                SincronizarRelacional(produtosRelacional, produtosNaoRelacional, produtostexto);
                SincronizarNaoRelacional(produtosRelacional, produtosNaoRelacional, produtostexto);
                SincronizarTexto(produtosRelacional, produtosNaoRelacional, produtostexto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SincronizarRelacional(List<int> relacional, List<int> naoRelacional, List<int> texto)
        {
            IEnumerable<int> difRelacionalNaoRelacional = naoRelacional.Except(relacional);
            IEnumerable<int> difRelacionalTexto = texto.Except(relacional);

            if (difRelacionalNaoRelacional?.Count() > 0)
            {
                foreach (int idDadoNaoRelacional in difRelacionalNaoRelacional)
                {
                    var produto = _produtoNaoRelacionalRepository.Recuperar(idDadoNaoRelacional);
                    produto.Id = 0;
                    _produtoRelacionalRepository.Criar(produto);
                }
            }

            if (difRelacionalTexto?.Count() > 0)
            {
                foreach (int idDadoTexto in difRelacionalTexto)
                {
                    var produto = _produtoXmlRepository.Recuperar(idDadoTexto);
                    produto.Id = 0;
                    _produtoRelacionalRepository.Criar(produto);
                }
            }
        }

        private void SincronizarNaoRelacional(List<int> relacional, List<int> naoRelacional, List<int> texto)
        {
            IEnumerable<int> difNaoRelacionalRelacional = relacional.Except(naoRelacional);
            IEnumerable<int> difNaoRelacionalTexto = texto.Except(naoRelacional);

            if (difNaoRelacionalRelacional?.Count() > 0)
            {
                foreach (int idDadoRelacional in difNaoRelacionalRelacional)
                {
                    var produto = _produtoRelacionalRepository.Recuperar(idDadoRelacional);
                    produto.Id = 0;
                    _produtoNaoRelacionalRepository.Criar(produto);
                }
            }

            if (difNaoRelacionalTexto?.Count() > 0)
            {
                foreach (int idDadoTexto in difNaoRelacionalTexto)
                {
                    var produto = _produtoXmlRepository.Recuperar(idDadoTexto);
                    produto.Id = 0;
                    _produtoNaoRelacionalRepository.Criar(produto);
                }
            }
        }

        private void SincronizarTexto(List<int> relacional, List<int> naoRelacional, List<int> texto)
        {
            IEnumerable<int> difTextoRelacional = relacional.Except(texto);
            IEnumerable<int> difTextoNaoRelacional = naoRelacional.Except(texto);

            if (difTextoRelacional?.Count() > 0)
            {
                foreach (int idDadoRelacional in difTextoRelacional)
                {
                    var produto = _produtoRelacionalRepository.Recuperar(idDadoRelacional);
                    produto.Id = 0;
                    _produtoXmlRepository.Criar(produto);
                }
            }

            if (difTextoNaoRelacional?.Count() > 0)
            {
                foreach (int idDadoNaoRelacional in difTextoNaoRelacional)
                {
                    var produto = _produtoNaoRelacionalRepository.Recuperar(idDadoNaoRelacional);
                    produto.Id = 0;
                    _produtoXmlRepository.Criar(produto);
                }
            }
        }

        private Produto VerificarDemaisBases(int id)
        {
            try
            {
                var produtoMongodb = _produtoNaoRelacionalRepository.Recuperar(id);
                if (produtoMongodb != null)
                {
                    PersistirDados((int)EnumRepositorios.NAO_RELACIONAL, produtoMongodb);
                    return produtoMongodb;
                }

                var produtoTexto = _produtoXmlRepository.Recuperar(id);
                if (produtoTexto != null)
                {
                    PersistirDados((int)EnumRepositorios.TEXTO, produtoTexto);
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
                case (int)EnumRepositorios.NAO_RELACIONAL:
                    produto.Id = 0;
                    _produtoRelacionalRepository.Criar(produto);
                    if (_produtoXmlRepository.Recuperar(produto.IdCompartilhado) == null)
                        _produtoXmlRepository.Criar(produto);
                    break;
                case (int)EnumRepositorios.TEXTO:
                    produto.Id = 0;
                    _produtoRelacionalRepository.Criar(produto);
                    _produtoNaoRelacionalRepository.Criar(produto);
                    break;
            }
        }
    }
}
