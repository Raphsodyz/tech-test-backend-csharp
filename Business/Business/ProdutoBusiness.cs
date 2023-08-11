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
            throw new NotImplementedException();
        }

        public void Criar(ProdutoDTO entidade)
        {
            throw new NotImplementedException();
        }

        public void Atualizar(ProdutoDTO produto)
        {
            throw new NotImplementedException();
        }

        public void Deletar(int? id)
        {
            throw new NotImplementedException();
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
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SincronizarBases()
        {
            try
            {
                List<int> produtosRelacional = _produtoRelacionalRepository.ListarDadosCompartilhados();
                List<int> produtosNaoRelacional = _produtoNaoRelacionalRepository.ListarDadosCompartilhados();
                List<int> produtostexto = _produtoXmlRepository.ListarDadosCompartilhados();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
