using Data.Repositories.NaoRelacional.Context;
using Data.Repositories.NaoRelacional.Interface;
using Domain.Entidades;
using MongoDB.Driver;

namespace Data.Repositories.NaoRelacional.Repository
{
    public class ProdutoNaoRelacionalRepository : IProdutoNaoRelacionalRepository
    {
        private readonly MobilusNaoRelacionalContext _context;
        public ProdutoNaoRelacionalRepository(MobilusNaoRelacionalContext context)
        {
            _context = context;
        }

        public IList<Produto> Listar(int? maximo)
        {
            var produtos = _context.RecuperaColecao();
            if (maximo != null)
                return produtos.AsQueryable().Take(maximo.Value).ToList();

            return produtos.AsQueryable().ToList();
        }

        public Produto Recuperar(Guid id)
        {
            return _context.RecuperaColecao().Find(p => p.IdCompartilhado == id).FirstOrDefault();
        }

        public void Atualizar(Produto produto)
        {
            var filtro = Builders<Produto>.Filter.Eq(p => p.IdCompartilhado, produto.IdCompartilhado);
            var exist = _context.RecuperaColecao().Find(filtro).FirstOrDefault();
            if (exist == null)
            {
                _context.RecuperaColecao().InsertOne(produto);
                return;
            }

            produto.Id = exist.Id;
            _context.RecuperaColecao().ReplaceOne(filtro, produto);
        }

        public void Criar(Produto produto)
        {
            produto.Id = Guid.NewGuid();
            var produtoExistente = _context.RecuperaColecao().Find(p => p.IdCompartilhado == produto.IdCompartilhado).FirstOrDefault();
            if (produtoExistente == null)
            {
                _context.RecuperaColecao().InsertOne(produto);
                return;
            }

            var filtro = Builders<Produto>.Filter.Eq(p => p.IdCompartilhado, produto.IdCompartilhado);
            _context.RecuperaColecao().ReplaceOne(filtro, produto);
        }

        public void Deletar(Guid id)
        {
            _context.RecuperaColecao().DeleteOne(p => p.IdCompartilhado == id);
        }

        public List<Guid> ListarDadosCompartilhados()
        {
            return _context.RecuperaColecao().AsQueryable().Select(p => p.IdCompartilhado)?.ToList();
        }
    }
}
