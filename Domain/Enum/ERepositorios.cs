using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum ERepositorios
    {
        [Display(Name = "RELACIONAL")]
        RELACIONAL = 0,
        [Display(Name = "NÃO RELACIONAL")]
        NAO_RELACIONAL = 1,
        [Display(Name = "TEXTO")]
        TEXTO = 2
    }
}
