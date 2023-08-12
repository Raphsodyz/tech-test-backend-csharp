using Data.Repositories.Relacional.Context;
using Data.Repositories.Relacional.Interface;
using Domain.Entidades;
using Microsoft.EntityFrameworkCore;


namespace Data.Repositories.Relacional.Repository
{
    public class ProdutoRelacionalRepository : IProdutoRelacionalRepository
    {
        public MobilusRelacionalContext _context;
        public DbSet<Produto> _dbSet;

        public ProdutoRelacionalRepository(MobilusRelacionalContext context)
        {
            _context = context;
            _dbSet = _context.Set<Produto>();
        }

        public IList<Produto> Listar(int? maximo)
        {
            if (maximo != null)
                _dbSet.Take(maximo.Value);

            return _dbSet.ToList();
        }

        public Produto Recuperar(int id)
        {
            return _dbSet.FirstOrDefault(p => p.IdCompartilhado == id);
        }

        public void Criar(Produto entidade)
        {
            if (entidade.Id == 0)
            {
                _dbSet.Add(entidade);
                _context.SaveChanges();
            }
            else
            {
                if (_context.Entry(entidade).State == EntityState.Detached)
                {
                    Produto dbEntity = _dbSet.Find(entidade.Id);
                    _context.Entry(dbEntity).CurrentValues.SetValues(entidade);
                }
                else if (_context.Entry(entidade).State == EntityState.Unchanged)
                    _context.Entry(entidade).State = EntityState.Modified;

                _context.SaveChanges();
            }
        }

        public void Atualizar(Produto produto)
        {
            if (_context.Entry(produto).State == EntityState.Detached)
            {
                Produto dbProduto = _dbSet.Find(produto.Id);
                _context.Entry(dbProduto).CurrentValues.SetValues(produto);
            }
            else if (_context.Entry(produto).State == EntityState.Unchanged)
                _context.Entry(produto).State = EntityState.Modified;

            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            Produto dbProduto = _dbSet.Find(id);

            if (_context.Entry(dbProduto).State == EntityState.Detached)
                _dbSet.Attach(dbProduto);

            _dbSet.Remove(dbProduto);
        }

        public List<int> ListarDadosCompartilhados()
        {
            return _dbSet.Select(p => p.IdCompartilhado)?.ToList();
        }

        public void SalvarAlteracoes()
        {
            _context.SaveChanges();
        }
    }
}
