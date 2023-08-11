using System;
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
        }
    }
}
