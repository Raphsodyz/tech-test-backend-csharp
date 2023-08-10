using Business.Interface;
using Data.MySQL.Interface;
using Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Business
{
    public class ProdutoBusiness : IProdutoBusiness
    {
        private readonly IProdutoRepository _produtoRepository;
        public ProdutoBusiness(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public void Atualizar(Produto produto)
        {
            throw new NotImplementedException();
        }

        public void Criar(Produto entidade)
        {
            throw new NotImplementedException();
        }

        public void Deletar(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Produto> Listar()
        {
            throw new NotImplementedException();
        }

        public Produto Recuperar(int id)
        {
            throw new NotImplementedException();
        }
    }
}
