using Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interface
{
    public interface IProdutoBusiness
    {
        IList<Produto> Listar();
        Produto Recuperar(int id);
        void Criar(Produto entidade);
        void Atualizar(Produto produto);
        void Deletar(int id);
    }
}
