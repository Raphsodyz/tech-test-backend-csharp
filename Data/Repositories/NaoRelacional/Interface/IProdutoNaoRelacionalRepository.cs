﻿using Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.NaoRelacional.Interface
{
    public interface IProdutoNaoRelacionalRepository
    {
        IList<Produto> Listar(int? maximo);
        Produto Recuperar(Guid id);
        void Criar(Produto entidade);
        void Atualizar(Produto produto);
        void Deletar(Guid id);
        List<Guid> ListarDadosCompartilhados();
    }
}
