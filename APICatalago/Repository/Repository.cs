using APICatalago.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace APICatalago.Repository
{
    public class Repository<T> : IRepository<T> where T : class // onde T eh uma classe, somente posso passar uma classe para meu repositorio
    {
        protected AppDbContext _context;

        public Repository(AppDbContext contexto)
        {
            _context = contexto;
        }

        public IQueryable<T> Get()
        {
            return _context.Set<T>().AsNoTracking(); //asnotracking permite desabilitar o rastreamento de entidade e assim ganhar desempenho
            //o metodo set do contexto retonar uma instancia dbset<t> para o acesso a entidades de determinado tipo no contexto
        }

        public async Task<T> GetById(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(predicate);
        }


        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified; // definindo que o estado como modificado
            _context.Set<T>().Update(entity);
        }
    }
}
