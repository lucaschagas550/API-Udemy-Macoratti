using APICatalago.Models;
using APICatalago.Repository.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalago.DTOs.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Produto, ProdutoDTO>().ReverseMap();

            CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        }
    }
}
