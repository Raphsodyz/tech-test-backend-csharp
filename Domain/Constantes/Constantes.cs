﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Constantes
{
    public class Constantes
    {

        public static class MensagensErro
        {
            public static readonly string ARGUMENTOS_VAZIOS = "Não foram enviados todos os dados para completar a requisição. Verifique e tente novamente. Campo: ";
            public static readonly string PRODUTO_NAO_ENCONTRADO = "O produto selecionado na busca não foi encontrado.";
            public static readonly string ERRO_500 = "O serviço está indisponível ou foi encontrado algum erro durante a execução de sua requisição.";
            public static readonly string XML_BLOQUEADO = "O arquivo XML se encontra bloquado. Não foi possível salvar os produtos que foram inclusos localmente.";
            public static readonly string LISTA_VAZIA = "Não existem produtos cadastrados em seu banco de dados.";
            public static readonly string ERRO_MONGODB = "Atenção! O dado enviado foi salvo no banco de dados relacional, porém não foi salvo no MongoDB e no .xml. Verifique a disponibilidade do MongoDB e após faça uma sincronização.";
            public static readonly string ERRO_XML = "Atenção! O dado enviado foi salvo no banco de dados relacional e não relacional, no arquivo .xml. Verifique a disponibilidade do arquivo .xml e após faça uma sincronização.";
        }

        public static class MensagensSucesso
        {
            public static readonly string PRODUTO_CRIADO = "Produto adicionado aos bancos de dados com sucesso. O código é: ";
            public static readonly string PRODUTO_ATUALIZADO = "O produto foi atualizado com sucesso.";
            public static readonly string PRODUTO_DELETADO = "O produto foi removido dos bancos de dados com sucesso.";
            public static readonly string PRODUTOS_SINCRONIZADOS = "Os produtos existentes nas bases de dados foram sincronizados com sucesso.";
        }
    }
}
