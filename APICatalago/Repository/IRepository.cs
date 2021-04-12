using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace APICatalago.Repository
{
    public interface IRepository<T> //significa que eh uma interface generica, para um tipo T comum
    {
        IQueryable<T> Get(); //  consultar, IQueryable permite chamadas Assincronas

        Task<T> GetById(Expression<Func<T, bool>> predicate); // consultar por ID, Task<T> permite chamadas Assincrona

        void Add(T entity); //Adicionar

        void Update(T entity); // Atualizar

        void Delete(T entity); // Deletar

    }
}
