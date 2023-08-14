using AutoMapper;
using Domain.DTO;
using Domain.Entidades;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Mapper
{
    public class MobilusMapper : Profile
    {
        public MobilusMapper()
        {
            CreateMap<Produto, ProdutoDetalhesDTO>()
                .ForMember(
                    dto => dto.ValorTotal,
                    entity => entity.MapFrom(src => src.Quantidade * src.Preco))
                .ForMember(
                    dto => dto.Codigo,
                    entity => entity.MapFrom(src => src.IdCompartilhado));
            CreateMap<ProdutoDTO, Produto>().ReverseMap();
        }
    }
}
