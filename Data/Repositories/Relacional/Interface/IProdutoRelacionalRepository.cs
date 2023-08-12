using Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Relacional.Interface
{
    public interface IProdutoRelacionalRepository
    {
        IList<Produto> Listar(int? maximo);
        Produto Recuperar(int id);
        void Criar(Produto entidade);
        void Atualizar(Produto produto);
        void Deletar(int id);
        List<int> ListarDadosCompartilhados();
        void SalvarAlteracoes();
    }
}
