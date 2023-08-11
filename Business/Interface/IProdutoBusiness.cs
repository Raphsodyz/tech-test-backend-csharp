﻿using Domain.DTO;
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
        IList<ProdutoDTO> Listar(int? maximo);
        ProdutoDTO Recuperar(int? id);
        void Criar(ProdutoDTO entidade);
        void Atualizar(ProdutoDTO produto);
        void Deletar(int? id);
        void SincronizarBases();
    }
}
