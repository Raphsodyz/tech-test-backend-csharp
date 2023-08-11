using Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.XmlTexto.Interfaces
{
    public interface IProdutoXmlRepository
    {
        IList<Produto> Listar(int? maximo);
        Produto Recuperar(int id);
        void Criar(Produto entidade);
        void Atualizar(Produto produto);
        void Deletar(int id);
        List<int> ListarDadosCompartilhados();

    }
}
