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

        public Produto Recuperar(int id)
        {
            return _context.RecuperaColecao().Find(p => p.IdCompartilhado == id).FirstOrDefault();
        }

        public void Atualizar(Produto produto)
        {
            var filtro = Builders<Produto>.Filter.Eq(p => p.IdCompartilhado, produto.IdCompartilhado);
            _context.RecuperaColecao().ReplaceOne(filtro, produto);
        }

        public void Criar(Produto produto)
        {
            var produtoExistente = _context.RecuperaColecao().Find(p => p.IdCompartilhado == produto.IdCompartilhado).FirstOrDefault();
            if (produtoExistente?.IdCompartilhado == 0)
                _context.RecuperaColecao().InsertOne(produto);

            var filtro = Builders<Produto>.Filter.Eq(p => p.IdCompartilhado, produto.IdCompartilhado);
            _context.RecuperaColecao().ReplaceOne(filtro, produto);
        }

        public void Deletar(int id)
        {
            _context.RecuperaColecao().DeleteOne(p => p.IdCompartilhado == id);
        }

        public List<int> ListarDadosCompartilhados()
        {
            return _context.RecuperaColecao().AsQueryable().Select(p => p.IdCompartilhado)?.ToList();
        }
    }
}
