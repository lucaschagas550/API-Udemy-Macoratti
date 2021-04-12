using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalago.Pagination
{
    public class PagedList<T> : List<T> // T significa que a classe é uma classe generica
    {
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPage;

        public PagedList(List<T> itens, int count,int pageNumber,int pageSize) //itens, total de itens, numero da pagian, tamanho da pagina
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPage = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(itens);
        }

        //paginação assincrona
        public async static Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber,int pageSize)// fonte da daos a paginar, page numero, e tamanho
        {
            var count = source.Count();
            var itens = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(itens, count, pageNumber, pageSize);
        }
    }
}
