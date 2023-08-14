using Domain.DTO;
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
        IList<ProdutoDetalhesDTO> Listar(int? maximo);
        ProdutoDetalhesDTO Recuperar(Guid? id);
        Guid Criar(ProdutoDTO produtoDTO);
        void Atualizar(Guid? id, ProdutoDTO produto);
        void Deletar(Guid? id);
        void SincronizarBases();
    }
}
