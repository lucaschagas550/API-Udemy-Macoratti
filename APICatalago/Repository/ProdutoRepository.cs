using APICatalago.Context;
using APICatalago.Models;
using APICatalago.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalago.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext contexto) : base(contexto)
        {

        }

        //SEM PAGINAÇÃO
        //public IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParameters) 
        //{
        //    return Get()
        //         .OrderBy(on => on.Nome)
        //         .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
        //         .Take(produtosParameters.PageSize)
        //         .ToList();
        //}
        
        //COM PAGINAÇÃO ASSINCRONA
        public async Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters)
        {
            return await PagedList<Produto>.ToPagedList(Get().OrderBy(on => on.ProdutoId),
                produtosParameters.PageNumber, produtosParameters.PageSize);
        }

        public async Task<IEnumerable<Produto>> GetProdutosPorPreco()
        {
            return await Get().OrderBy(c => c.Preco).ToListAsync();    
        }

    }
}
