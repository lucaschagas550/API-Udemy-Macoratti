using APICatalago.Models;
using APICatalago.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalago.Repository
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        //IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParameters); // sem paginação

        Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters); // para paginação 
        Task<IEnumerable<Produto>> GetProdutosPorPreco();
    }
}
