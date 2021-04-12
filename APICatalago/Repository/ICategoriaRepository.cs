using APICatalago.Models;
using APICatalago.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalago.Repository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        //IQueryble
        Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriasParameters);
        Task<IEnumerable<Categoria>> GetCategoriasProdutos();
    }
}
