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
            return produtos.Find(p => true).ToList();
        }
    }
}
