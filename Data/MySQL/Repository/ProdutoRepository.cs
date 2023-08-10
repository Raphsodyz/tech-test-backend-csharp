using Data.MySQL.Context;
using Data.MySQL.Interface;
using Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.MySQL.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        public MobilusContext _context;
        public DbSet<Produto> _dbSet;

        public ProdutoRepository(MobilusContext context) 
        {
            _context = context;
            _dbSet = _context.Set<Produto>();
        }

        public IList<Produto> Listar()
        {
            return _dbSet.ToList();
        }

        public Produto Recuperar(int id)
        {
            return _dbSet.FirstOrDefault(p => p.Id == id);
        }

        public void Criar(Produto entidade)
        {
            _dbSet.Add(entidade);
            _context.SaveChanges();
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
    }
}
